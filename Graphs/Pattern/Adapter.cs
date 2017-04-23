using System.Collections.Generic;

namespace Graphs.Pattern
{
    abstract class Adapter
    {
        public abstract int[,] Transform();
    }

    internal class CliqueMatrixAdapter : Adapter
    {
        readonly int[,] _cliqueMatrix;

        public CliqueMatrixAdapter(int[,] m)
        {
            _cliqueMatrix = (int[,])m.Clone();
        }

        public override int[,] Transform()
        {
            //массив m*n
            var retV = new int[_cliqueMatrix.GetLength(1), _cliqueMatrix.GetLength(1)];
            var n = _cliqueMatrix.GetLength(1);
            for (var k = 0; k < _cliqueMatrix.GetLength(0); k++)
            {
                var ta = new List<int>();
                for (var j = 0; j < n; j++)
                    if (_cliqueMatrix[k, j] == 1)
                        ta.Add(j);
                for (var i = 0; i < ta.Count - 1; i++)
                {
                    for (var j = i + 1; j < ta.Count; j++)
                    {
                        retV[ta[i], ta[j]] = 1;
                    }
                }
            }
            for (var i = 0; i < retV.GetLength(0) - 1; i++)
            {
                for (var j = i + 1; j < retV.GetLength(1); j++)
                {
                    retV[j, i] = retV[i, j];
                }
            }
            return retV;
        }
    }
}
