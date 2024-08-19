# XMLBunny

This is a tool that parses an XML file and generates an Excel format report detailing the structure of the file.

This currently consists of a console application built in C#.

## To Get Started

1. Download `XMLBunny.zip` from the assets of the latest release or click <a href="https://github.com/Health-Informatics-UoN/xml-bunny/releases/download/v1.0.0/XMLBunny.zip">here</a>.

2. Extract all from `XMLBunny.zip`.

3. Open the command line interface. To do this:
    - Click on `windows + R` on the keyboard.
    - Type in `'cmd'` and press the `Enter` key.

4. From the command line, `cd path-to-XMLBunny-extracted-folder` e.g `cd Downloads/XMLBunny`

5. Type in `'XMLBunny.exe'` and press the `Enter` key to run the executable.

6. Once running, follow the instructions to parse XML files with desired requirements.

## Sample

- XML File `(sample.xml)`

```<root>
<doc>
<person>
  <name>
    <firstName>Emma</firstName>
    <lastName>Brown</lastName>
  </name>
  <email>emma.brown@code-maze.com</email>
  <age>58</age>
</person>
<person>
  <name>
    <firstName>Emma</firstName>
    <lastName>Brown</lastName>
  </name>
  <email>emma.brown@code-maze.com</email>
  <age>70</age>
</person>
</doc>
```

- XMLBunny Console

![Screenshot 2024-08-19 220005](https://github.com/user-attachments/assets/39f50e9c-ace7-408b-b796-2f3b119cc552)

- Output File `(sample.xlsx)`

![Screenshot 2024-08-19 220249](https://github.com/user-attachments/assets/4414cd93-4c63-457c-b1d5-4fb569b5d6e2)
