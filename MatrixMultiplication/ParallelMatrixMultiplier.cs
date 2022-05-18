using MatrixMultiplication.MultiThreaded;

namespace MatrixMultiplication;

public abstract class ParallelMatrixMultiplier
{
    protected double[,] FirstMatrix { get; }
    protected double[,] SecondMatrix { get; }

    protected ParallelMatrixMultiplier(double[,] firstMatrix, double[,] secondMatrix)
    {
        FirstMatrix = firstMatrix;
        SecondMatrix = secondMatrix;
    }

    public abstract double[,] Multiply(int threadNumber);

    public double[,] GroupBlocksIntoMatrix(MatrixBlock[,] blocks)
    {
        var size = blocks[0, 0].MatrixPartC.GetLength(1);
        var result = new double[blocks.GetLength(0) * size, blocks.GetLength(1) * size];

        for (var blockI = 0; blockI < blocks.GetLength(0); blockI++)
        {
            for (var blockJ = 0; blockJ < blocks.GetLength(1); blockJ++)
            {
                var currentPart = blocks[blockI, blockJ].MatrixPartC;

                for (var i = 0; i < currentPart.GetLength(0); i++)
                    for (var j = 0; j < currentPart.GetLength(1); j++)
                        result[blockI * size + i, blockJ * size + j] = currentPart[i, j];
            }
        }
        return result;
    }
}