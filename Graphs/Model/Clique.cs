using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphs.Model
{
    class Clique : IEnumerable<DataVertex>, IEnumerable, ICloneable, IComparable<Clique>
    { 
        List<DataVertex> vertexList;

        public Clique()
        {
            vertexList = new List<DataVertex>();
        }

        public Clique(List<DataVertex> vl)
        {
            vertexList = new List<DataVertex>(vl);
        }

        public int Count
        {
            get { return vertexList.Count; }
        }

        public DataVertex this[int index]
        {
            get { return vertexList[index]; }
            set { vertexList.Insert(index, value); }
        }

        /// <summary>
        /// Добавление вершины в клику
        /// </summary>
        /// <param name="v"></param>
        public void Add(DataVertex v)
        {
            vertexList.Add(v);
        }

        /// <summary>
        /// Добавление набора вершин в клику
        /// </summary>
        /// <param name="c"></param>
        public void AddClique(Clique c)
        {
            vertexList.AddRange(c.vertexList);
        }

        /// <summary>
        /// Удаление вершин из клики
        /// </summary>
        /// <param name="c"></param>
        public void RemoveVertices(string c)
        {
            vertexList.RemoveAll(x => x.Text == c);
        }

        /// <summary>
        /// Упорядочивание вершин в клике
        /// </summary>
        /// <returns></returns>
        public Clique OrderByID()
        {
            var orderedList = vertexList.OrderBy(x => x.ID).ToList();
            var clique = new Clique(orderedList);
            return clique;
        }

        #region IEnumerable

        public IEnumerator<DataVertex> GetEnumerator()
        {
            return vertexList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region ICloneable

        public object Clone()
        {
            return new List<DataVertex>(vertexList);
        }

        #endregion

        #region IComparable

        public int CompareTo(Clique other)
        {
            int compareResult = 0;

            if (other != null)
            {
                if (vertexList.Count == other.Count)
                {
                    for (var i = 0; i < vertexList.Count; i++)
                    {
                        if (vertexList[i].Angle == other[i].Angle &&
                           vertexList[i].Color == other[i].Color &&
                           vertexList[i].E == other[i].E &&
                           vertexList[i].GroupId == other[i].GroupId &&
                           vertexList[i].SkipProcessing == other[i].SkipProcessing &&
                           vertexList[i].ID == other[i].ID &&
                           vertexList[i].Text == other[i].Text)
                        {
                            compareResult = 1;
                        }
                        else
                        {
                            compareResult = 0;
                            break;
                        }
                    }
                }
                else
                    compareResult = 0;
            }
            else
                compareResult = 0;

            return compareResult;
        }

        #endregion
    }
}
