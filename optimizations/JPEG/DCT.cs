using System;
using System.Threading.Tasks;

namespace JPEG;

public class DCT
{
	private readonly int _dctSize;
	private readonly double[,] _basisMatrix;
	private readonly double[,] _basisMatrixT;

	public DCT(int dctSize = 8)
	{
		_dctSize = dctSize;
		_basisMatrix = new double[_dctSize, _dctSize];
		_basisMatrixT = new double[_dctSize, _dctSize];
		
		for (var i = 0; i < _dctSize; i++)
		for (var j = 0; j < _dctSize; j++)
		{
			var ci = i == 0 ? Math.Sqrt(dctSize) : Math.Sqrt(dctSize / 2d);
			_basisMatrix[i, j] = 1 / ci * Math.Cos(Math.PI / dctSize * (0.5 + j) * i);
			_basisMatrixT[j, i] = _basisMatrix[i, j];
		}
	}
	
	public double[,] DCT2D(double[,] input)
	{
		var height = input.GetLength(0);
		var width = input.GetLength(1);
		
		if (height != _dctSize || width != _dctSize) throw new ArgumentException(
			$"Dimensions don't match: {width}x{height} and {_dctSize}x{_dctSize}");
		
		var leftPart = MatrixMultiply(_basisMatrix, input);
		return MatrixMultiply(leftPart, _basisMatrixT);
	}

	public double[,] IDCT2D(double[,] input)
	{
		var height = input.GetLength(0);
		var width = input.GetLength(1);
		
		if (height != _dctSize || width != _dctSize) throw new ArgumentException(
        			$"Dimensions don't match: {width}x{height} and {_dctSize}x{_dctSize}");
		
		var leftPart = MatrixMultiply(_basisMatrixT, input);
		return MatrixMultiply(leftPart, _basisMatrix);
	}

	private static double[,] MatrixMultiply(double[,] matrix1, double[,] matrix2)
	{
		var result = new double[matrix1.GetLength(0), matrix2.GetLength(1)];
		
		for (var i = 0; i < result.GetLength(0); i++)
		for (var j = 0; j < matrix2.GetLength(1); j++)
		for (var k = 0; k < matrix1.GetLength(1); k++)
			result[i, j] = matrix1[i, k] * matrix2[k, j];
		
		return result;
	}
}