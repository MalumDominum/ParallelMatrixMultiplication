namespace MatrixMultiplication.MultiThreaded;

public class MatrixBlock
{
    public double[,] MatrixPartA { get; set; }
    public double[,] MatrixPartB { get; set; }
    public double[,] MatrixPartC { get; private set; }
    public double[,]? CurrentMatrixPartA { get; set; }

    public MatrixBlock(double[,] matrixPartA, double[,] matrixPartB)
    {
        MatrixPartA = matrixPartA;
        MatrixPartB = matrixPartB;
        MatrixPartC = new double[matrixPartA.GetLength(0), matrixPartB.GetLength(1)];
    }

    public bool UpdateResult(bool neededSum = true)
    {
        if (CurrentMatrixPartA == null) return false;
        MatrixPartC = neededSum ? MatrixTools.Sum(MatrixPartC, MatrixTools.Multiply(CurrentMatrixPartA, MatrixPartB)) : 
                                  MatrixTools.Multiply(CurrentMatrixPartA, MatrixPartB);
        return true;
    }
}
