using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphs.Model
{
    class Clique : IEnumerable<DataVertex>, ICloneable, IComparable<Clique>
    {
        readonly List<DataVertex> _vertexList;

        public Clique()
        {
            _vertexList = new List<DataVertex>();
        }

        public Clique(List<DataVertex> vl)
        {
            _vertexList = new List<DataVertex>(vl);
        }

        public int Count => _vertexList.Count;

        public DataVertex this[int index]
        {
            get => _vertexList[index];
            set => _vertexList.Insert(index, value);
        }

        /// <summary>
        /// Добавление вершины в клику
        /// </summary>
        /// <param name="v"></param>
        public void Add(DataVertex v)
        {
            _vertexList.Add(v);
        }

        /// <summary>
        /// Добавление набора вершин в клику
        /// </summary>
        /// <param name="c"></param>
        public void AddClique(Clique c)
        {
            _vertexList.AddRange(c._vertexList);
        }

        /// <summary>
        /// Удаление вершин из клики
        /// </summary>
        /// <param name="c"></param>
        public void RemoveVertices(string c)
        {
            _vertexList.RemoveAll(x => x.Text == c);
        }

        /// <summary>
        /// Упорядочивание вершин в клике
        /// </summary>
        /// <returns></returns>
        public Clique OrderByID()
        {
            var orderedList = _vertexList.OrderBy(x => x.ID).ToList();
            var clique = new Clique(orderedList);
            return clique;
        }

        #region IEnumerable

        public IEnumerator<DataVertex> GetEnumerator()
        {
            return _vertexList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region ICloneable

        public object Clone()
        {
            return new List<DataVertex>(_vertexList);
        }

        #endregion

        #region IComparable

        public int CompareTo(Clique other)
        {
            int compareResult = 0;

            if (other != null)
            {
                if (_vertexList.Count == other.Count)
                {
                    for (var i = 0; i < _vertexList.Count; i++)
                    {
                        if (Math.Abs(_vertexList[i].Angle - other[i].Angle) < 0.0001 &&
                           _vertexList[i].Color == other[i].Color &&
                           _vertexList[i].E == other[i].E &&
                           _vertexList[i].GroupId == other[i].GroupId &&
                           _vertexList[i].SkipProcessing == other[i].SkipProcessing &&
                           _vertexList[i].ID == other[i].ID &&
                           _vertexList[i].Text == other[i].Text)
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
