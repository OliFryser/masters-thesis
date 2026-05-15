import sys
import os
import re

import matplotlib.pyplot as plt


class Entry:
    def __init__(self, sigma, fitness):
        self.sigma: float = sigma
        self.fitness: float = fitness
        self.convergence: int = 5250


entries: list[Entry] = []

domain_name = re.search(r"(\w+)_.+", sys.argv[1])

if not domain_name:
    raise ValueError("Invalid path for argument 1")

savepathPrefix = os.path.join("plots", domain_name.group(1))

with open(sys.argv[1], "r") as file:
    for line in file:
        line = line.strip()
        numbers = line.split(",")
        sigma = float(numbers[0].split(":")[1])
        fitness = float(numbers[1].split(":")[1])

        entries.append(Entry(sigma, fitness))

entries.sort(key=lambda e: e.sigma)


with open(sys.argv[2], "r") as file:
    for line in file:
        line = line.strip()
        # Iteration 0: MAP-Elites converged at iteration 500. Archive Size: 7. Max fitness 1
        if "converged" in line:
            iter_match = re.search(r"Iteration (\d+)", line)
            conv_match = re.search(r"converged at iteration (\d+)", line)

            if iter_match and conv_match:
                iteration_number = int(iter_match.group(1))
                convergence_number = int(conv_match.group(1))

                entries[iteration_number].convergence = convergence_number

plt.figure(figsize=(8, 5))

plt.plot(
    list(map(lambda e: e.sigma, entries)),
    list(map(lambda e: e.fitness, entries)),
    marker="o",
    linestyle="-",
    color="b",
)

plt.xlabel("Sigma ($\sigma$)", fontsize=12)
plt.ylabel("Global Reliability", fontsize=12)

plt.grid(True, linestyle="--", alpha=0.6)
plt.xlim(0, 1)
plt.ylim(0, 1)

plt.savefig(savepathPrefix + "_fitness_plot.png")

plt.figure(figsize=(8, 5))

plt.plot(
    list(map(lambda e: e.sigma, entries)),
    list(map(lambda e: e.convergence, entries)),
    marker="o",
    linestyle="-",
    color="b",
)

plt.xlabel("Sigma ($\sigma$)", fontsize=12)
plt.ylabel("Converged at iteration", fontsize=12)

plt.grid(True, linestyle="--", alpha=0.6)
plt.xlim(0, 1)
plt.ylim(0, 5250)

plt.savefig(savepathPrefix + "_convergence_plot.png")
