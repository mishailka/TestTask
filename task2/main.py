import sys
import math

def main():
    if len(sys.argv) != 3:
        print("Использование: python main.py путь_к_файлу_окружности.txt путь_к_файлу_точек.txt")
        sys.exit(1)

    # считывание координат центра окружности а так же её радиус
    with open(sys.argv[1], 'r') as f:
        x, y = map(int, f.readline().split())
        r = int(f.readline())

    # считывание и обработка
    with open(sys.argv[2], 'r') as f:
        for line in f:
            if line.strip():
                px, py = map(int, line.split())
                distance = math.sqrt((px - x)**2 + (py - y)**2) # расчет расстояния до центра окружности
                if math.isclose(distance, r):
                    print(0)
                elif distance < r:
                    print(1)
                else:
                    print(2)

if __name__ == "__main__":
    main()