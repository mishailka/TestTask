import sys
"""
функция ищет самый дешевый путь по изменению всех элеметов к одному 
принимает числовой массив, возвращает число 
"""
def find_cheapest_path(numarr):
    if not numarr or not isinstance(numarr,list): #проверка на существование переданного элемента и на то
                                                # массив ли это
        return 0
    cheapest_path = None  #инициация для первого сравнения

    for each_elem in numarr:
        temp_path = 0 # тут хранится стоимость превидения всех элементов к этому
        for other_each_elem in numarr:
            temp_path += abs(each_elem-other_each_elem) #записываем сколько нужно чтобы привести один элемент ко второму
        if cheapest_path is None or cheapest_path > temp_path:
            cheapest_path = temp_path
    return cheapest_path if cheapest_path is not None else 0


#просто считывание файла из txt
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
        print("Использование: python main.py"
              " <путь_к_файлу.txt> ")
        sys.exit(1)
    file_path = sys.argv[1]
    print(find_cheapest_path(read_numbers_from_file(file_path)))