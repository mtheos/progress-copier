# progress-copier
A file copier that displays current progress for copying large files.

#### Example usage
```cs
var pb = new AsciiProgressBar();
var fc = new ProgressFileCopier(pb);
pb.ProgressChanged += (percentage, bar) => {
                            Console.SetCursorPosition(0, Console.CursorTop);
                            Console.Write($"What a beautiful bar {bar} {percentage:P0} in my line :)");
                        };
pb.Completed += delegate { Console.WriteLine("\nDone"); };
fc.Copy("src.dat", "dst.dat", true);
```
