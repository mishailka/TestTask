import sys

"""
принимает в себя два целых числа
первый аргумент задает длинну масива от 1 до n
второй задает шаг
"""
def find_array_path(n, m):
    path = []
    curr = 1
    while str(curr) not in path:
        path.append(str(curr))
        curr = (curr + m - 1) % n   # находим конец следующего по шагу массива
        if curr == 0: # т.к массив от 1 до n, а выше я хожу по кругу при помощи остатка от деления то на конце массива получется 0 а не конец
            curr = n
    return ''.join(path)


if __name__ == "__main__":
    if len(sys.argv) != 3:
        print("Использование: python main.py <n> <m>")
        sys.exit(1)
    n = int(sys.argv[1])
    m = int(sys.argv[2])


    print(find_array_path(n, m))