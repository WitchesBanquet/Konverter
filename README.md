# Konverter - 通用文本样式转换器

# 开发中

## 功能

Konverter 可以通过自定义的模版，将一种结构的文本内容转换为另外一种结构的文本。

输入模版采用一种结构性的模版语言（自行开发的），输出模版采用 Liquid 模版语言。

- 可自定义的输入模版
- 文件名也可以使用模版来获取信息
- 可自定义的输出模版
- 输出模版可添加 Index 目录模版
- 输出模版采用 Liquid 模版语言，支持所有 Liquid 语法
- 可以快速进行大批量操作

## 基本应用举例

假设有下面这样的源文件，一本小说的第一卷，有 40 个章节，即有 40 个文件：

```markdown
这是书的标题
    第一卷 第3章 章节标题
    
    BlaBlaBla
    正文内容

日期
作者署名
```

可以构建出这样的输入模版（变量名必须是在 C# 中合法的）：

```markdown
{$ Line $}{| Title:str |}
{$ Line $}{| Volume:str |} 第{| Chapter:int |}章 {| ChapterTitle:str |}
{$ Section $}{| Content:str |}
{$ Line $}{| Date |}
{$ Line $}{| Author |}
```

关于模版的语法，请参考 [模版语法(TODO)]()

假设要下面这样的输出：

```markdown
这是书的标题
    第一卷
    第3章 章节标题
    作者署名 - 日期

    BlaBlaBla
    正文内容
```

可以使用 Liquid 模版语言编写出这样的输出模版：

```liquid
{{ Title }}
    {{ Volume }}
    第{{ Chapter }}章 {{ ChapterTitle }}
    {{ Author }} - {{ Date }}

    {{ Content }}
```

通过 Konverter，可以实现快速的批量转换。

