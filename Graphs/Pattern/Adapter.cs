using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphs.Pattern
{
    abstract class Adapter
    {
        public abstract int[,] Transform();
    }

    class CliqueMatrixAdapter : Adapter
    {
        int[,] cliqueMatrix;

        public CliqueMatrixAdapter(int[,] m)
        {
            cliqueMatrix = (int[,])m.Clone();
        }

        public override int[,] Transform()
        {
            //массив m*n
            int[,] RetV = new int[cliqueMatrix.GetLength(1), cliqueMatrix.GetLength(1)];
            int n = cliqueMatrix.GetLength(1);
            for (int k = 0; k < cliqueMatrix.GetLength(0); k++)
            {
                List<int> TA = new List<int>();
                for (int j = 0; j < n; j++)
                    if (cliqueMatrix[k, j] == 1)
                        TA.Add(j);
                for (int i = 0; i < TA.Count - 1; i++)
                {
                    for (int j = i + 1; j < TA.Count; j++)
                    {
                        RetV[TA[i], TA[j]] = 1;
                    }
                }
            }
            for (int i = 0; i < RetV.GetLength(0) - 1; i++)
            {
                for (int j = i + 1; j < RetV.GetLength(1); j++)
                {
                    RetV[j, i] = RetV[i, j];
                }
            }
            return RetV;
        }
    }
}
