from matplotlib import pyplot as plt

from plotting_utils import *


class Entry:
    def __init__(self, x: int, y: int, fitness: float, feasibility: float, is_feasible):
        self.x = x
        self.y = y
        self.fitness = fitness
        self.feasibility = feasibility
        self.is_feasible = is_feasible


class BehaviorSpacePlotter:

    def __init__(self, text_file: str, output_path: str):
        self.text_file = text_file
        self.output_path = output_path
        
        self.entries: list[Entry] = []

        self.buckets_per_axis: int = 0
        self.behavior_x_label: str = ""
        self.behavior_y_label: str = ""
        
        self.__parse_text_file()

    def __parse_text_file(self):
        text_file_path = os.path.join(self.output_path, self.text_file)
        try:
            with open(text_file_path, "r", encoding="utf-8") as f:
                f.readline() # skip header
                self.buckets_per_axis = int(f.readline().strip())
                f.readline() # skip header
                self.behavior_x_label = f.readline().strip()
                self.behavior_y_label = f.readline().strip()
                f.readline() # skip header
                
                for line in f:
                    if not line.strip(): continue
                    split = line.split()
                    x = int(split[0].strip())
                    y = int(split[1].strip())
                    fitness = float(split[2].strip())
                    feasibility = float(split[3].strip())
                    self.entries.append(Entry(x, y, fitness, feasibility, fitness > 0))
                    

        except Exception as e:
                eprint(f"An error occured: {e}")


    def plot_archive_coverage(self):
        fig_name = get_figname(self.text_file)
        output_file_path = os.path.join(self.output_path, f"{fig_name}.png")
        fitness_map = [[float('nan') for _ in range(self.buckets_per_axis)] for _ in range(self.buckets_per_axis)]
        feasibility_map = [[float('nan') for _ in range(self.buckets_per_axis)] for _ in range(self.buckets_per_axis)]
        
        for entry in self.entries:
            if entry.is_feasible:
                fitness_map[entry.y][entry.x] = entry.fitness

        for entry in self.entries:
            if not entry.is_feasible:
                feasibility_map[entry.y][entry.x] = entry.feasibility

        plt.figure(figsize=(12, 8))
        ax = plt.gca()
        
        x_max = 0.2
        y_max = 1.0
        
        extent = [0, x_max, 0, y_max]


        # Draw fit entries
        img_fitness = ax.imshow(
            fitness_map, 
            cmap='viridis', 
            interpolation='nearest', 
            origin='lower', 
            vmin=0.0, 
            vmax=1.0, 
            aspect='auto',
            extent=extent)

        # Draw infeasible entries
        # 'gray' colormap with low alpha makes unfeasible cells look "ghosted" or dimmed
        img_feasibility = ax.imshow(
            feasibility_map,
            cmap='gist_gray_r',
            interpolation='nearest',
            origin='lower',
            extent=extent,
            aspect='auto',
            vmin=0.0,
            vmax=1.0,
            alpha=0.5,
            zorder=2)
        
        # Draw the grid
        x_grid = np.linspace(0, x_max, self.buckets_per_axis + 1)
        y_grid = np.linspace(0, y_max, self.buckets_per_axis + 1)

        ax.vlines(x=x_grid, ymin=0, ymax=y_max, color='black', linewidth=0.3, alpha=0.7, zorder=3)
        ax.hlines(y=y_grid, xmin=0, xmax=x_max, color='black', linewidth=0.3, alpha=0.7, zorder=3)
                
        # Draw major ticks
        ax.set_xticks(np.linspace(0, x_max, 5), minor=False)
        ax.set_yticks(np.linspace(0, y_max, 6), minor=False)

        # Colorbars
        plt.colorbar(img_fitness, label='Fitness')
        plt.colorbar(img_feasibility, label='Feasibility')
        
        plt.xlabel(self.behavior_x_label)
        plt.ylabel(self.behavior_y_label)
        plt.title('Behavior Space')

        plt.savefig(output_file_path, dpi=300, bbox_inches='tight')
        plt.close()
    
    
    