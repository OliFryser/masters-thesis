import os
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


def generate_ticks(start, end, step):
    ticks = []
    current = start
    while current <= end:
        ticks.append(round(current, 4))
        current += step
    return ticks