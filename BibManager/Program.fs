open System
open System.IO
open System.Text.RegularExpressions

open BibtexLibrary

let private printEntry number (entry : BibtexEntry) =
    let getKey k = 
        match entry.Tags.TryGetValue k with
        | true, v -> v
        | false, _ -> ""

    let title = getKey "title"
    let ``type`` = entry.Type
    let info =
        match getKey "journal" with
        | "" -> getKey "booktitle"
        | other -> other
    let data = sprintf "%s, %s: %s, %s" info (getKey "address") (getKey "publisher") (getKey "year")
    let authors = (getKey "author").Replace (" and ", ", ")
    let entryDefinition = sprintf "| %d | %s | %s | %s | %s |" (number + 1) title ``type`` data authors
    Console.WriteLine entryDefinition

let private printFile (file : BibtexFile) =
    Seq.iteri printEntry file.Entries

[<EntryPoint>]
let main argv = 
    let fileName = Array.exactlyOne argv
    
    let text = File.ReadAllText fileName
    let bibtex = Regex.Replace (text, "^%.*$", "", RegexOptions.Multiline) // TODO: Will not be needed after new BibtexLibrary release ~ F
    let replaced = bibtex.Replace("``", "\"").Replace("''", "\"").Replace("–", "---").Replace("«", "\'").Replace("»", "\"").Replace("№", "No") // TODO: See https://github.com/MaikelH/BibtexLibrary/issues/6
    let parsed = BibtexImporter.FromString replaced
    printFile parsed

    0
