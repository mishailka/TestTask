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
        curr = (curr + m - 1) % n   #находим конец следующего по шагу массива
        if curr == 0: # т.к массив от 1 до n, а выше я хожу по кругу при помощи остатка от деления то на конце массива получется 0 а не конец
            curr = n
    return ''.join(path)


if __name__ == "__main__":
    if len(sys.argv) != 3:
        print("Использование: python task1.py <n> <m>")
        sys.exit(1)

    try:
        n = int(sys.argv[1])
        m = int(sys.argv[2])
    except ValueError:
        print("Ошибка: n и m должны быть целыми числами")
        sys.exit(1)

    print(find_array_path(n, m))