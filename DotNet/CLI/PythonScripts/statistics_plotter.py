import matplotlib.pyplot as plt
from plotting_utils import *
from behavior_space_plotter import BehaviorSpacePlotter


def create_plot_from_text_file(text_file_path: str, output_folder_path):
    figname = get_figname(text_file_path)
    output_file_path = os.path.join(output_folder_path, f"{figname}.png")
    header_to_data: dict[str, list[float]] = parse_text_file(
        os.path.join(output_folder_path, text_file_path)
    )

    # plt.figure(figsize=(10, 6))

    for header, values in header_to_data.items():
        if values:
            plt.plot([i for i in range(len(values))], values, label=header)

    plt.title(figname)
    plt.xlabel("Iteration")
    plt.ylabel(figname)
    plt.grid(True, linestyle=':', alpha=0.6)
    plt.legend()

    plt.savefig(output_file_path)
    plt.close()

    print(f"Successfully saved plot to: {output_file_path}")


def create_feasibility_plot(text_file_path: str, output_folder_path: str) -> None:
    figname = get_figname(text_file_path)
    output_file_path = os.path.join(output_folder_path, f"{figname}.png")
    header_to_data: dict[str, list[float]] = parse_text_file(
        os.path.join(output_folder_path, text_file_path)
    )
    
    pop_size_header = "Population Size"
    pop_size_points = header_to_data[pop_size_header]
    feasible_pop_header = "Feasible Population Size"
    feasible_pop_points = header_to_data[feasible_pop_header]
    iterations = [x for x in range(len(pop_size_points))]

    plt.plot(iterations, pop_size_points, label=pop_size_header, color='black', linestyle='--')
    plt.plot(iterations, feasible_pop_points, label=feasible_pop_header, color='blue')
    plt.fill_between(iterations, feasible_pop_points, color='blue', alpha=0.3)

    plt.xlabel('Generations')
    plt.ylabel('Population Size')
    plt.title('Population vs. Feasibility')
    plt.legend()
    plt.grid(True, linestyle=':', alpha=0.6)
    plt.savefig(output_file_path)
    plt.close()
    
    print(f"Successfully saved plot to: {output_file_path}")


def get_text_files(folder_path: str) -> list[str]:
    txt_file_paths: list[str] = []

    if not os.path.exists(folder_path):
        print(f"Error: The path '{folder_path}' does not exist.")
        return []

    for root, _, files in os.walk(folder_path):
        for file in files:
            if file.lower().endswith(".txt"):
                txt_file_paths.append(file)

    return txt_file_paths


if __name__ == "__main__":
    path = sys.argv[1]
    text_files = get_text_files(path)
    for text_file in text_files:
        if "Feasibility" in text_file:
            create_feasibility_plot(text_file, path)
        elif "BehaviorSpace" in text_file:
            behavior_space_plotter = BehaviorSpacePlotter(text_file, path)
            behavior_space_plotter.plot_archive_coverage()
        else:
            create_plot_from_text_file(text_file, path)
