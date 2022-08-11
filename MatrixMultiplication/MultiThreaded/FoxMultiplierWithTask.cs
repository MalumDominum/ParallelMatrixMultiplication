namespace MatrixMultiplication.MultiThreaded;

public class FoxMultiplierWithTask : FoxMultiplierWithThread
{
    public FoxMultiplierWithTask(double[,] matrixA, double[,] matrixB) : base(matrixA, matrixB) { }

    public override double[,] Multiply(int threadNumber)
    {
        if (FirstMatrix.GetLength(0) != SecondMatrix.GetLength(1) && FirstMatrix.GetLength(1) != SecondMatrix.GetLength(0))
            throw new ArithmeticException($"Can't multiply matrices with sizes {FirstMatrix.GetLength(0)}x{FirstMatrix.GetLength(1)} " +
                                          $"and {SecondMatrix.GetLength(0)}x{SecondMatrix.GetLength(1)}");

        var stepsCount = (int)Math.Sqrt(threadNumber);
        if (threadNumber != Math.Pow(stepsCount, 2))
            throw new ArgumentException("You must use numbers from which you can take the square root");

        var subMatrixSize = Math.Sqrt(Math.Pow(FirstMatrix.GetLength(0), 2) / threadNumber);
        if (subMatrixSize % 1 != 0)
            throw new ArithmeticException($"Can't divide matrix with size {FirstMatrix.GetLength(0)}x{FirstMatrix.GetLength(1)} " +
                                          $"on {threadNumber} equal parts");

        var blocks = InitBlocks(stepsCount);
        for (var iter = 0; iter < stepsCount; iter++)
        {
            var threads = new Task[threadNumber];
            for (var i = 0; i < stepsCount; i++)
            {
                var j = (i + iter) % stepsCount;
                var currentMatrixA = blocks[i, j].MatrixPartA;
                for (var k = 0; k < stepsCount; k++)
                {
                    var block = blocks[i, k];
                    block.CurrentMatrixPartA = currentMatrixA;
                    var thread = new Task(() => block.UpdateResult());
                    thread.Start();
                    threads[i * stepsCount + k] = thread;
                }
            }
            Task.WaitAll(threads);
            for (var j = 0; j < blocks.GetLength(1); j++)
            {
                var startMatrixB = blocks[0, j].MatrixPartB;

                for (var i = 0; i < blocks.GetLength(0) - 1; i++)
                    blocks[i, j].MatrixPartB = blocks[i + 1, j].MatrixPartB;

                blocks[blocks.GetLength(0) - 1, j].MatrixPartB = startMatrixB;
            }
        }
        return GroupBlocksIntoMatrix(blocks);
    }
}