from matplotlib import pyplot as plt

from plotting_utils import *


class Entry:
    def __init__(self, x: int, y: int, fitness: float):
        self.x = x
        self.y = y
        self.fitness = fitness


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
                    self.entries.append(Entry(x, y, fitness))
                    

        except Exception as e:
                eprint(f"An error occured: {e}")


    def plot_archive_coverage(self):
        fig_name = get_figname(self.text_file)
        output_file_path = os.path.join(self.output_path, f"{fig_name}.png")
        behavior_map = [[float('nan') for _ in range(self.buckets_per_axis)] for _ in range(self.buckets_per_axis)]
        
        for entry in self.entries:
            behavior_map[entry.y][entry.x] = entry.fitness

        plt.figure(figsize=(6, 6))
        
        img = plt.imshow(behavior_map, cmap='viridis', interpolation='nearest', origin='lower', vmin=0.0, vmax=1.0)
        
        plt.colorbar(img, label='Fitness Value')
        plt.xlabel(self.behavior_x_label)
        plt.ylabel(self.behavior_y_label)
        plt.title('Behavior Space')
        
        plt.xticks(ticks=range(self.buckets_per_axis + 1), labels=generate_ticks(0.0, 1.0, 1.0 / self.buckets_per_axis))
        plt.yticks(ticks=range(self.buckets_per_axis + 1), labels=generate_ticks(0.0, 1.0, 1.0 / self.buckets_per_axis))
        
        plt.savefig(output_file_path)
    
    
    