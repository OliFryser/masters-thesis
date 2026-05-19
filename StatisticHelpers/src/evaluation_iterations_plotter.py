import json
import os
import sys
import matplotlib.pyplot as plt

plt.rcParams.update({"font.size": 12, "axes.grid": True, "grid.alpha": 0.3})

json_lines: list[str] = []
with open(sys.argv[1], "r") as file:
    for line in file:
        json_lines.append(line)

data = json.loads("".join(json_lines))

iterations = []
fitness_values = []
feasibility_values = []

for block in data:
    n = block["EvaluationIterations"]
    iterations.append(f"M={n}")

    f_deltas = [entry["Fitness"] for entry in block["Entries"]]
    p_deltas = [entry["Feasibility"] for entry in block["Entries"]]

    fitness_values.append(f_deltas)
    feasibility_values.append(p_deltas)

# fitness delta
_, ax1 = plt.subplots(figsize=(7, 6))

box1 = ax1.boxplot(
    fitness_values, tick_labels=iterations, patch_artist=True, widths=0.4
)
for patch in box1["boxes"]:
    patch.set_facecolor("#ffb703")
    patch.set_edgecolor("#fb8500")
for median in box1["medians"]:
    median.set_color("#1d3557")
    median.set_linewidth(2)

ax1.axhline(0, color="#2b2d42", linestyle="-", alpha=0.6)
ax1.set_title("Evaluation Iterations Impact on Fitness Values", fontsize=16)
ax1.set_ylabel("Fitness", fontsize=14)
ax1.set_xlabel("Evaluation Iterations", fontsize=14)

plt.savefig(
    os.path.join("plots", "fitness_comparison.png"), dpi=300, bbox_inches="tight"
)
plt.show()

# feasiblity delta
_, ax2 = plt.subplots(figsize=(7, 6))

box2 = ax2.boxplot(
    feasibility_values, tick_labels=iterations, patch_artist=True, widths=0.4
)

for patch in box2["boxes"]:
    patch.set_facecolor("#8ecae6")
    patch.set_edgecolor("#219ebc")
for median in box2["medians"]:
    median.set_color("#1d3557")
    median.set_linewidth(2)

ax2.axhline(0, color="#2b2d42", linestyle="-", alpha=0.6)
ax2.set_title("Evaluation Iteration Impact on Feasibility Values", fontsize=16)
ax2.set_ylabel("Feasibility", fontsize=14)
ax2.set_xlabel("Evaluation Iterations", fontsize=14)

plt.savefig(
    os.path.join("plots", "feasibility_comparison.png"), dpi=300, bbox_inches="tight"
)
plt.show()
