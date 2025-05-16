import sys

def find_cheapest_path(numarr):
    if not numarr or not isinstance(numarr,list):
        return 0
    cheapest_path = None

    for each_elem in numarr:
        temp_path = 0
        for other_each_elem in numarr:
            temp_path += abs(each_elem-other_each_elem)
        if cheapest_path is None or cheapest_path > temp_path:
            cheapest_path = temp_path
    return cheapest_path if cheapest_path is not None else 0


def read_numbers_from_file(file_path):
    numbers = []
    with open(file_path, 'r', encoding='utf-8') as file:
        for line in file:
            line = line.strip()
            if line:
                try:
                    num = int(line)
                    numbers.append(num)
                except ValueError:
                    pass
    return numbers


if __name__ == "__main__":
    if len(sys.argv) != 2:
        print("Использование: python task4.py"
              " <путь_к_файлу.txt> ")
        sys.exit(1)
    file_path = sys.argv[1]
    print(find_cheapest_path(read_numbers_from_file(file_path)))