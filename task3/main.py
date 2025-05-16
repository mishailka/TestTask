import json
import sys

#функция для чтения джисонок
def load_file(filename):
    with open(filename, 'r', encoding='utf-8') as file:
        return json.load(file)

"""
функция для генерации данных для репорта, принимает данные из test.json и values.json
"""
def fill_data(test_data, values_data):

    values_map = {item['id']:item['value'] for item in values_data['values']} #словарь вида ID:значение.
                                                                              # Взято из values.json
    for test in test_data["tests"]:
        if test["id"] in values_map:                 #если айдишка теста есть в считанном выше словаре
                                                     # то записываем значение values из словаря
            test["value"] = values_map[test["id"]]
    return test_data

if __name__ == "__main__":
    if len(sys.argv) != 4:
        print("Использование: python task3.py"
              " <путь_к_tests.json> "
              "<путь_к_values.json>"
              " <путь_к_report.json>")
        sys.exit(1)
    test_file, values_file, report_file = sys.argv[1], sys.argv[2], sys.argv[3]

    test_data = load_file(test_file)
    values_data = load_file(values_file)

    report_data = fill_data(test_data,values_data)

    with open(report_file,"w",encoding="utf-8") as file:
        json.dump(report_data,file,indent=2,ensure_ascii=False)
        