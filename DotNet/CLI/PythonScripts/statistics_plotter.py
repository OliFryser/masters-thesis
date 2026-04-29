import os

import matplotlib.pyplot as plt
import sys


def eprint(*args, **kwargs):
    print(*args, file=sys.stderr, **kwargs)


def parse_text_file(text_file_path: str) -> dict[str, list[float]]:
    header_to_data: dict[str, list[float]] = {}
    current_header = None

    try:
        with open(text_file_path, "r", encoding="utf-8") as f:
            for line in f:
                line = line.strip()
                if not line:
                    continue

                try:
                    value = float(line)
                    if current_header:
                        header_to_data[current_header].append(value)
                except ValueError:
                    # It's a Header
                    current_header = line
                    if current_header not in header_to_data:
                        header_to_data[current_header] = []

    except Exception as e:
        eprint(f"An error occured: {e}")

    return header_to_data

def get_figname(text_file_path) -> str:
    return os.path.splitext(text_file_path)[0]

def create_plot_from_text_file(text_file_path: str, output_folder_path):
    figname = get_figname(text_file_path)
    output_file_path = os.path.join(output_folder_path, f"{figname}.png")
    header_to_data: dict[str, list[float]] = parse_text_file(
        os.path.join(output_folder_path, text_file_path)
    )

    plt.figure(figsize=(10, 6))

    for header, values in header_to_data.items():
        if values:
            plt.plot([i for i in range(len(values))], values, label=header)

    plt.title(figname)
    plt.xlabel("Iteration")
    plt.ylabel(figname)
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
    plt.fill_between(iterations, pop_size_points, color='blue', alpha=0.3)

    plt.xlabel('Generations')
    plt.ylabel('Population Size')
    plt.title('MAP-Elites: Population vs. Feasibility')
    plt.legend()
    plt.grid(True, linestyle=':', alpha=0.6)

    plt.savefig('population_growth_feasibility.png')


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
            create_feasbility_plot(text_file, path)
        create_plot_from_text_file(text_file, path)
