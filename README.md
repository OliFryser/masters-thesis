# Infinimon

Improving Reliability and Consistency of the [Wave Function Collapse](https://github.com/mxgmn/WaveFunctionCollapse) algorithm.

![Examples of maps generated](Docs/Images/PalletTownExample.png).

## Description

This repository contains multiple tools to explore the kinds of levels WFC can generate from a given tilemap. It is currently a proof of concept, so the usability is not a top priority.

The pipeline we offer is the following:

1. Analyse a tilemap into a tileset and adjacency rules.
2. Use the *InfinimonCLI* to generate an archive of input to Wave Function Collapse.
3. Use the *Archive Explorer* to explore the archive of input to WFC showing different levels belonging to a specific part of the behavior space.

We believe this pipeline can be of help to game developers in *mixed-initiative PCG*, where the designers works and the algorithm works in collaboration to create content.

For a more detailed explanation of this project, read [our thesis report]().

### The InfinimonCLI tool

A command line interface program that can create an archive of weights for WFC. The command line program can be run either from the binary or built from the project in the folder ``DotNet/CLI``.

#### Command Line Switches Reference

The CLI supports a number of command line switches. The deafult Run Mode is Constrained MAP-Elites (no switch).

| Long Switch        | Short Switch | Category      | Notes                                                                                                  |
| :----------------- | :----------- | :------------ | :----------------------------------------------------------------------------------------------------- |
| `--skip-stats`     | `-s`         | Statistics    | Disables statistics generation.                                                                        |
| `--regular`        | `-r`         | Run Mode      | Standard MAP-Elites run mode.                                                                          |
| `--hyper`          | `-h`         | Run Mode      | Used for hyperparameter tuning.                                                                        |
| `--iterations`     | `-i`         | Run Mode      | Runs comparisons evaluation iteration.                                                                 |
| `--domain=[int]`   | `-d=[int]`   | Configuration | Sets the problem domain. 0 = Letter, 1 = Arrow, 2 = Pokémon (default)                                  |
| `--mutation=[int]` | `-m=[int]`   | Configuration | Sets the mutation strategy. 0 = All Tiles (default), 1 = 1/3 Tiles, 2 = Special Tile Only, 3 = CMA-CME |
| `--entropy`        | `-e`         | Behavior      | Changes variation behavior to entropy-based calculation.                                               |

### The Archive Explorer

The Archive Explorer can be launched from the Unity build or from the Unity project itself. It allows 

### Unity Project

## Prerequisites

TODO

## Getting Started

TODO

## Why and when?

This project is the Master's Thesis of [Oliver Juhl Friis Hansen](https://olifrys.dk) and [Willaim Skou Heidemann](https://www.linkedin.com/in/william-skou-heidemann-8b7b4019b/) for their MSc in Games Technology from the IT-University of Copenhagen in 2026.
