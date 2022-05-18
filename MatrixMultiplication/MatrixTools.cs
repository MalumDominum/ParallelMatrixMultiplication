namespace MatrixMultiplication;

public static class MatrixTools
{
    private static readonly Random Rnd = new();

    public static double[,] Generate(int width, int height)
    {
        var matrix = new double[width, height];

        const int rangeMin = -1000;
        const int rangeMax = 1000;

        for (var i = 0; i < width; i++)
            for (var j = 0; j < height; j++)
                matrix[i, j] = rangeMin + (rangeMax - rangeMin) * Rnd.NextDouble();

        return matrix;
    }

    public static double[,] Multiply(double[,] firstMatrix, double[,] secondMatrix)
    {
        var result = new double[firstMatrix.GetLength(0), secondMatrix.GetLength(1)];

        for (var row = 0; row < result.GetLength(0); row++)
            for (var col = 0; col < result.GetLength(1); col++)
                result[row, col] = MultiplyMatricesCell(firstMatrix, secondMatrix, row, col);

        return result;
    }

    private static double MultiplyMatricesCell(double[,] firstMatrix, double[,] secondMatrix, int row, int col)
    {
        double cell = 0;

        for (var i = 0; i < secondMatrix.GetLength(0); i++)
            cell += firstMatrix[row, i] * secondMatrix[i, col];

        return cell;
    }

    public static double[,] Sum(double[,] firstMatrix, double[,] secondMatrix)
    {
        var result = new double[firstMatrix.GetLength(0), firstMatrix.GetLength(1)];

        for (var i = 0; i < firstMatrix.GetLength(0); i++)
            for (var j = 0; j < firstMatrix.GetLength(1); j++)
                result[i, j] = firstMatrix[i, j] + secondMatrix[i, j];

        return result;
    }

    public static double[,] GetPart(double[,] matrix, int indexI, int indexJ, int width, int height)
    {
        var result = new double[height, width];

        for (var i = 0; i < height; i++)
            for (var j = 0; j < width; j++)
                result[i, j] = matrix[i + indexI, j + indexJ];

        return result;
    }

    public static bool CompareMatrices(double[,] firstMatrix, double[,] secondMatrix)
    {
        if (firstMatrix.GetLength(0) != secondMatrix.GetLength(0) &&
            firstMatrix.GetLength(1) != secondMatrix.GetLength(1))
            return false;

        for (var i = 0; i < firstMatrix.GetLength(0); i++)
            for (var j = 0; j < firstMatrix.GetLength(1); j++)
                if (Math.Abs(firstMatrix[i, j] - secondMatrix[i, j]) > 0.000001)
                    return false;

        return true;
    }

    public static void PrintMatrix(double[,] matrix)
    {
        for (var i = 0; i < matrix.GetLength(0); i++)
            for (var j = 0; j < matrix.GetLength(1); j++)
                Console.Write(j != matrix.GetLength(1) ? matrix[i, j] + ", " : matrix[i, j] + "\n");
    }
}