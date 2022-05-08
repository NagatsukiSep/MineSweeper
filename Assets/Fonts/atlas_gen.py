from dis import code_info

file_name = "unique_char.txt"

with open(file_name, "r", encoding="utf-8") as f:
    原文 = f.read()
原文 = 原文.replace('\n', '').replace(' ', '').replace(
    '\t', '').replace('\r', '').replace('\u3000', '')

重複文字列 = []
for i in 原文:
    重複文字列.append(i)
一意文字列 = list(set(重複文字列))
一意文字列.sort()
一意文字列を文字列にしたやつ = "".join(s for s in 一意文字列)
for i in range(len(一意文字列)):
    print(一意文字列[i], end="")
with open(file_name, "w", encoding="utf-8") as f:
    f.write(一意文字列を文字列にしたやつ)
