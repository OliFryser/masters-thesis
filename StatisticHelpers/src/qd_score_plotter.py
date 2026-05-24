import os
import sys

from matplotlib import pyplot as plt


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


folder = sys.argv[1]
parsedFitnessFile = parse_text_file(os.path.join(folder, "Data", "Fitness.txt"))
parsedReliabilityFile = parse_text_file(os.path.join(folder, "Data", "Reliability.txt"))
fitness_values: list[float] = parsedFitnessFile["Max Fitness"]
reliability_values: list[float] = parsedReliabilityFile["Reliability"]

qd_scores: list[float] = []

for i in range(len(reliability_values)):
    qd_scores.append(reliability_values[i] * fitness_values[i])

plt.title("Quality-Diversity Score", fontsize=20)

plt.plot([i for i in range(len(qd_scores))], qd_scores)

plt.xticks(fontsize=12)
plt.yticks(fontsize=12)

plt.xlabel("Iteration", fontsize=14)
plt.ylabel("QD-Score", fontsize=14)
plt.grid(True, linestyle=":", alpha=0.6)

plt.savefig(os.path.join(folder, "QDScore.png"), bbox_inches="tight")
plt.close()
