using Graphs.Model;
using System.Collections.Generic;
using System.Linq;

namespace Graphs.Algorithms
{
    internal class BronKerbosch
    {
        private readonly List<DataEdge> _edges;

        public List<Clique> Cliques { get; set; }

        public BronKerbosch(IEnumerable<DataEdge> e)
        {
            _edges = new List<DataEdge>(e);
            Cliques = new List<Clique>();
        }

        /// <summary>
        /// Поиск всех клик графа
        /// </summary>
        /// <param name="R"></param>
        /// <param name="P"></param>
        /// <param name="X"></param>
        public void FindClique(Clique R, List<DataVertex> P, List<DataVertex> X)
        {
            if (P.Count < 1 && X.Count < 1)
            {
                var clique = new Clique(new List<DataVertex>(R));
                Cliques.Add(clique);
            }

            foreach (var v in P)
            {
                var r = new Clique(new List<DataVertex>(R)) {v};

                var p = P.FindAll(ver => EdgeExists(ver, v));
                var x = X.FindAll(ver => EdgeExists(ver, v));
                FindClique(r, p, x);

                p.Remove(v);
                x.Add(v);
            }
        }

        #region Воспомогательные функции

        /// <summary>
        /// Соединения между вершиной и всеми вершинами из множества
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool HasEdges(DataVertex vertex, List<DataVertex> list)
        {
            //соединена ли вершина со всеми кандидатами
            var success = false;
            foreach (var v in list)
            {
                if (_edges.All(x => (x.Source.ID == vertex.ID && x.Target.ID == v.ID) || (x.Source.ID == v.ID && x.Target.ID == vertex.ID))) 
                {
                    success =  true;
                }
            }
            return success;
        }

        /// <summary>
        /// Существование ребра между двумя вершинами
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public bool EdgeExists(DataVertex v1, DataVertex v2)
        {
            //есть ли ребро между двумя вершинами
            var success = _edges.Any(x => (x.Source.ID == v1.ID && x.Target.ID == v2.ID) || (x.Source.ID == v2.ID && x.Target.ID == v1.ID));
            return success;
        }

        /// <summary>
        /// Удаление повторяющихся клик
        /// </summary>
        /// <returns></returns>
        public List<Clique> RemoveDuplicatedLists()
        {
            var tempList = new List<Clique>();

            //упорядочим клики по убыванию количества вершин
            var sourceList = new List<Clique>(Cliques);
            var orderedSourceList = new List<Clique>();

            while(sourceList.Count != 0)
            {
                var clique = sourceList.Find(x => x.Count() == sourceList.Max(y=>y.Count()));
                sourceList.Remove(clique);
                orderedSourceList.Add(clique);               
            }

            foreach (var l in orderedSourceList)
            {
                //упорядочиваем клики по возрастанию номеров вершин
                var orderedSublist = l.OrderByID();

                //проверяем дублирование клики
                if(tempList.All(item => item.CompareTo(orderedSublist) == 0))
                {
                    tempList.Add(l);
                }
            }

            return tempList;
        }
        
        /// <summary>
        /// Сравнение двух клик
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(List<DataVertex> x, List<DataVertex> y)
        {
            if (x.Count != y.Count)
                return false;
            for (var i = 0; i < x.Count; i++)
                if (x[i].ID != y[i].ID)
                    return false;
            return true;
        }

        #endregion
    }
}
