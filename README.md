# progress-copier
A file copier that displays current progress for copying large files.

#### Example usage
```cs
IProgressBar cpb = new ConsoleProgressBar();
IFileCopier pfc = new ProgressFileCopier(cpb);
cpb.ProgressChanged += (percentage, bar) => {
                            Console.SetCursorPosition(0, Console.CursorTop);
                            Console.Write($"What a beautiful bar {bar} {percentage:P0} in my line :)");
                        };
cpb.Completed += delegate { Console.WriteLine("\nDone"); };
pfc.Copy("src.dat", "dst.dat", true);
```
