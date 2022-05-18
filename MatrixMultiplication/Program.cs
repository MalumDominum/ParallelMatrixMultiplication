using MatrixMultiplication;
using MatrixMultiplication.MultiThreaded;
using MatrixMultiplication.Timing;

const int simpleCalculationLimit = 6912; //10000
var matrixSizes = new [] { 5184, 6912, 9216 }; // 576, 1152, 1728, 3456, 5184, 6912, 9216
var threadNumbers = new[] { 4, 9, 36, 64, 144 }; // 4, 9, 36, 64, 144

//var matrixSizesThatDivides = new List<int>();
//for (int i = 0; i < 30000; i++)
//{
//    var flag = true;
//    foreach (var t in threadNumbers)
//        if (i % t != 0)
//            flag = false;

//    if (flag) matrixSizesThatDivides.Add(i);
//}

var parallelTimeResults = new Tuple<double, double>[matrixSizes.Length, threadNumbers.Length];
var simpleTimeResults = new double[matrixSizes.Length];

for (var i = 0; i < matrixSizes.Length; i++)
{
    var firstMatrix = MatrixTools.Generate(matrixSizes[i], matrixSizes[i]);
    var secondMatrix = MatrixTools.Generate(matrixSizes[i], matrixSizes[i]);
    var results = new List<double[,]>();

    var timer = new TimeRecorder();
    ParallelMatrixMultiplier multiplier;
    Console.WriteLine($"Multiplications with size {matrixSizes[i]}:");

    if (matrixSizes[i] >= simpleCalculationLimit) //<= simpleCalculationLimit
    {
        results.Add(timer.RecordSpentTime(() => MatrixTools.Multiply(firstMatrix, secondMatrix))); //MatrixTools.Multiply(firstMatrix, secondMatrix)
        Console.WriteLine($"Time spent on Singlethread multiplication: {timer.TimeElapsed} ms");
        simpleTimeResults[i] = timer.TimeElapsed;
    }

    Console.WriteLine("Time spent on parallel multiplications");
    for (var j = 0; j < threadNumbers.Length; j++)
    {
        Console.WriteLine($"and {threadNumbers[j]} threads used:");

        multiplier = new FoxMultiplier(firstMatrix, secondMatrix);
        results.Add(timer.RecordSpentTime(() => multiplier.Multiply(threadNumbers[j]))); //multiplier.Multiply(threadNumbers[j])
        var foxTime = timer.TimeElapsed;
        Console.WriteLine($"\tFox multiplier: {foxTime} ms; speedup: {simpleTimeResults[i] / foxTime}");

        multiplier = new BlockStripedMultiplier(firstMatrix, secondMatrix);
        results.Add(timer.RecordSpentTime(() => multiplier.Multiply(threadNumbers[j]))); //multiplier.Multiply(threadNumbers[j])
        parallelTimeResults[i, j] = new Tuple<double, double>(foxTime, timer.TimeElapsed);
        Console.WriteLine($"\tBlock Striped multiplier: {timer.TimeElapsed} ms; speedup: {simpleTimeResults[i] / timer.TimeElapsed}");
    }


    //for ( var k = 0; k < results.Count; k++)
    //    results[k] = MatrixTools.GetPart(results[k], 100, 100, 100, 100);

    //foreach (var res in results)
    //{
    //    Console.WriteLine("\n");
    //    MatrixTools.PrintMatrix(res);
    //}

    //for (var resI = 0; resI < results.Count; resI++)
    //    for (var resJ = 0; resJ < results.Count; resJ++)
    //        if (resI != resJ)
    //            if (!MatrixTools.CompareMatrices(results[resI], results[resJ]))
    //                throw new Exception("Matrix not equals - some calculating went wrong.");
}

using StreamWriter file1 = new("Algorithm thread Results.csv");
for (var i = 0; i < matrixSizes.Length; i++)
{
    file1.Write($"\nSize:,{matrixSizes[i]},,Singlethread,{simpleTimeResults[i]}\n,");
    
    foreach (var threadNum in threadNumbers)
        file1.Write(threadNum + ",");

    file1.WriteLine();

    file1.Write("Fox,");
    for (var j = 0; j < threadNumbers.Length; j++)
        file1.Write($"{parallelTimeResults[i, j].Item1},");
    file1.WriteLine();

    file1.Write("Block Striped,");
    for (var j = 0; j < threadNumbers.Length; j++)
        file1.Write($"{parallelTimeResults[i, j].Item2},");
    file1.WriteLine();
}

using StreamWriter file2 = new("Algorithm size Results.csv");
for (var i = 0; i < threadNumbers.Length; i++)
{
    file2.Write($"\nThreads:,{threadNumbers[i]},,Singlethread,{simpleTimeResults[i]}\n,");

    foreach (var mSize in matrixSizes)
        file2.Write(mSize + ",");

    file2.WriteLine();

    file2.Write("Fox,");
    for (var j = 0; j < matrixSizes.Length; j++)
        file2.Write($"{parallelTimeResults[j, i].Item1},");
    file2.WriteLine();

    file2.Write("Block Striped,");
    for (var j = 0; j < matrixSizes.Length; j++)
        file2.Write($"{parallelTimeResults[j, i].Item2},");
    file2.WriteLine();
}