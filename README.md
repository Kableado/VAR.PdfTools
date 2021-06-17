# .Net library and tool to work with PDF files

## Usage

### VAR.PdfTools
Add the resulting assembly as reference in your projects, and this line on code:

```csharp
using VAR.PdfTools;
```

Then extract the contents of a data column using:

```csharp
var columnData = new List<string>();
PdfDocument doc = PdfDocument.Load("document.pdf");
foreach (PdfDocumentPage page in doc.Pages)
{
    PdfTextExtractor extractor = new PdfTextExtractor(page);
    columnData.AddRange(extractor.GetColumnAsStrings("Column"));
}
```

Or the content of a field (text on the right of the indicated text):

```csharp
var fieldData = new List<string>();
PdfDocument doc = PdfDocument.Load("document.pdf");
foreach (PdfDocumentPage page in doc.Pages)
{
    PdfTextExtractor extractor = new PdfTextExtractor(page);
    fieldData.Add(extractor.GetFieldAsString(txtFieldName.Text));
}
```

### VAR.PdfTools.Workbench
It is a simple Windows.Forms application, to test basic funcitionallity of the library.

## Building
A Visual Studio solution is provided. Simply, click build on the IDE.

The build generates a DLL and a Nuget package.

## Contributing
1. Fork it!
2. Create your feature branch: `git checkout -b my-new-feature`
3. Commit your changes: `git commit -am 'Add some feature'`
4. Push to the branch: `git push origin my-new-feature`
5. Submit a pull request :D

## Credits
* Valeriano Alfonso Rodriguez.

