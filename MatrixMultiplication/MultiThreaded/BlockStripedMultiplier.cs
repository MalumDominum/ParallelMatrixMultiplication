namespace MatrixMultiplication.MultiThreaded;

public class BlockStripedMultiplier : ParallelMatrixMultiplier
{
    public BlockStripedMultiplier(double[,] matrixA, double[,] matrixB) : base(matrixA, matrixB) { }
    
    public override double[,] Multiply(int threadNumber)
    {
        if (FirstMatrix.GetLength(0) != SecondMatrix.GetLength(1) && FirstMatrix.GetLength(1) != SecondMatrix.GetLength(0))
            throw new ArithmeticException($"Can't multiply matrices with sizes {FirstMatrix.GetLength(0)}x{FirstMatrix.GetLength(1)} " +
                                          $"and {SecondMatrix.GetLength(0)}x{SecondMatrix.GetLength(1)}");

        if (FirstMatrix.GetLength(0) % threadNumber != 0 || threadNumber > FirstMatrix.GetLength(0) || threadNumber < 1 || 
            FirstMatrix.GetLength(1) % threadNumber != 0 || threadNumber > FirstMatrix.GetLength(1))
            throw new ArithmeticException($"Can't divide matrix with size {FirstMatrix.GetLength(0)}x{FirstMatrix.GetLength(1)} " +
                                          $"on {threadNumber} equal parts");

        var allBlocks = new MatrixBlock[threadNumber, threadNumber];
        var blocks = InitBlocks(threadNumber);
        for (var currentIter = 0; currentIter < threadNumber; currentIter++)
        {
            for (var i = 0; i < blocks.Length; i++)
                allBlocks[i, (currentIter + 1) % threadNumber] = blocks[i];

            var threads = new Thread[threadNumber];
            for (var i = 0; i < threadNumber; i++)
            {
                var block = blocks[i];
                block.CurrentMatrixPartA = block.MatrixPartA;
                var thread = new Thread(() => block.UpdateResult(false));
                thread.Start();
                threads[i] = thread;
            }
            foreach (var thread in threads)
            {
                try { thread.Join(); }
                catch (ThreadInterruptedException e) { Console.WriteLine(e.StackTrace); }
            }

            var newBlocks = new MatrixBlock[threadNumber];
            for (var i = 0; i < threadNumber; i++)
                newBlocks[i] = new MatrixBlock(blocks[i].MatrixPartA, blocks[i].MatrixPartB);
            blocks = newBlocks;
        }
        return GroupBlocksIntoMatrix(allBlocks);
    }

    private MatrixBlock[] InitBlocks(int iterations)
    {
        var result = new MatrixBlock[iterations];
        var step = FirstMatrix.GetLength(1) / iterations;
        for (var i = 0; i < iterations; i++)
        {
            var matrixPartA = MatrixTools.GetPart(FirstMatrix, i * step, 0, FirstMatrix.GetLength(0), step);
            var matrixPartB = MatrixTools.GetPart(SecondMatrix, 0, i * step, step, SecondMatrix.GetLength(0));
            result[i] = new MatrixBlock(matrixPartA, matrixPartB);
        }
        return result;
    }
}