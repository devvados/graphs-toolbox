using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Graphs.Pattern;
using MahApps.Metro.Controls;
using QuickGraph;
using System.IO;
using System.Windows.Input;
using GraphX.Controls;
using Graphs.Model;

namespace Graphs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        /// <summary>
        /// Фабрика
        /// </summary>
        private Painter _creator;

        /// <summary>
        /// Строитель
        /// </summary>
        private Director _director;

        /// <summary>
        /// Для управления командами
        /// </summary>
        private readonly List<Command> _commands = new List<Command>();

        private Command _currentCommand;
        private int _commandCounter = -1;

        public MainWindow()
        {
            InitializeComponent();

            //Элементы на поле отрисовки графа
            var dgLogic = new GraphLogic();
            GraphArea.LogicCore = dgLogic;
            GraphArea.VertexSelected += GraphArea_VertexSelected;

            ZoomCtrl.IsAnimationEnabled = true;

            Painter.GraphZone = GraphArea;
            //Command.Graph = _graphWithCliques;
            DeleteCliqueCommand.Sp = SpDeletedClicks;
        }

        #region Конструирование графов

        /// <summary>
        /// Конструирование таблицы связности
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public int[,] BuildMatrix(int type)
        {
            int[,] Data2D = new int[,] { };

            switch (type)
            {
                case 0:
                    try
                    {
                        var filename = GetNameOfFileWithGraph();

                        if (filename != "")
                        {
                            Data2D = (int[,])(ReadGraphFromFile(filename)).Clone();
                        }
                        else
                            throw new Exception("Не выбран файл для загрузки графа!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка!");
                    }
                    break;
                case 1:
                    Data2D = new[,]
                    {
                        {0, 1, 1, 0},
                        {1, 0, 1, 1},
                        {1, 1, 0, 0},
                        {0, 1, 0, 0}
                    };
                    break;
                case 2:
                    Data2D = new[,]
                    {
                        { 0, 1, 1, 1 },
                        { 1, 0, 1, 1 },
                        { 1, 1, 0, 1 },
                        { 1, 1, 1, 0 }
                    };
                    break;
                case 3:
                    Data2D = new[,]
                    {
                        { 0, 1, 1, 0, 1, 1 },
                        { 1, 0, 1, 1, 0, 1 },
                        { 1, 1, 0, 1, 1, 0 },
                        { 0, 1, 1, 0, 1, 1 },
                        { 1, 0, 1, 1, 0, 1 },
                        { 1, 1, 0, 1, 1, 0 }
                    };
                    break;
                case 4:
                    Data2D = new[,]
                    {
                        { 0, 1, 0, 0, 0, 0, 1, 1 },
                        { 1, 0, 1, 0, 0, 0, 0, 1 },
                        { 0, 1, 0, 1, 0, 0, 0, 1 },
                        { 0, 0, 1, 0, 1, 0, 0, 1 },
                        { 0, 0, 0, 1, 0, 1, 0, 1 },
                        { 0, 0, 0, 0, 1, 0, 1, 1 },
                        { 1, 0, 0, 0, 0, 1, 0, 1 },
                        { 1, 1, 1, 1, 1, 1, 1, 0 }
                    };
                    break;
                case 5:
                    Data2D = new[,]
                    {
                        { 0, 1, 0, 0, 1, 0, 1, 0, 0, 0 },
                        { 1, 0, 1, 0, 0, 0, 0, 1, 0, 0 },
                        { 0, 1, 0, 1, 0, 0, 0, 0, 1, 0 },
                        { 0, 0, 1, 0, 1, 0, 0, 0, 0, 1 },
                        { 1, 0, 0, 1, 0, 1, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 1, 0, 0, 1, 1, 0 },
                        { 1, 0, 0, 0, 0, 0, 0, 0, 1, 1 },
                        { 0, 1, 0, 0, 0, 1, 0, 0, 0, 1 },
                        { 0, 0, 1, 0, 0, 1, 1, 0, 0, 0 },
                        { 0, 0, 0, 1, 0, 0, 1, 1, 0, 0 }
                    };
                    break;
            }

            return Data2D;
        }

        private void RBSelectGraph_Checked(object sender, RoutedEventArgs e)
        {
            var selectedGraph = Convert.ToInt32((sender as RadioButton)?.Tag);
            var m = BuildMatrix(selectedGraph);

            if (m.Length > 1)
            {
                _commands.Clear();
                _commandCounter = -1;

                //сборка графа из таблицы связности
                Builder builder = new GraphBuilder();
                _director = new Director(builder);
                _director.Construct(m);
                Command.Graph = builder.GetResult();

                //отрисовка графа
                _creator = new GraphPainter(Command.Graph);
                _creator.Draw();

                SpDeletedClicks.Children.Clear();
            }
        }

        #endregion

        #region Команды

        /// <summary>
        /// Команда НАЗАД
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BUndo_Click(object sender, RoutedEventArgs e)
        {
            if (_commandCounter > -1 && (_commands.Count - _commandCounter) < 4)
            {
                _commands[_commandCounter].UnExecute();
                _commandCounter--;
            }
        }

        /// <summary>
        /// Команда ВПЕРЕД
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BRedo_Click(object sender, RoutedEventArgs e)
        {
            if ((_commands.Count - _commandCounter < 5) && (_commands.Count - _commandCounter > 1))
            {
                _commandCounter++;
                _commands[_commandCounter].Execute();
            }
        }

        #endregion

        #region Работа с кликами

        private void BDeleteCliques_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (GraphArea.Children.Count > 0)
                {
                    while (_commands.Count - _commandCounter > 1)
                        _commands.RemoveAt(_commandCounter + 1);

                    //Непосредственно удаление
                    _currentCommand = new DeleteCliqueCommand(1);
                    _currentCommand.Execute();

                    _commands.Add(_currentCommand);
                    _commandCounter++;
                }
                else
                    throw new Exception("На поле отсутствует граф для анализа!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }

        private void BDeleteParallelCliques_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (GraphArea.Children.Count > 0)
                {
                    while (_commands.Count - _commandCounter > 1)
                        _commands.RemoveAt(_commandCounter + 1);

                    //Непосредственно удаление
                    _currentCommand = new DeleteCliqueCommand(2);
                    _currentCommand.Execute();

                    _commands.Add(_currentCommand);
                    _commandCounter++;
                }
                else
                    throw new Exception("На поле отсутствует граф для анализа!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }

        private void BBuildByCliqueMatrix_Click(object sender, RoutedEventArgs e)
        {
            int[,] cliqueMatrix = new[,]
            {
                { 1,0,0,1,0,1 },
                { 1,1,1,0,0,0 },
                { 0,1,0,0,1,0 }
            };

            CliqueMatrixAdapter adapter = new CliqueMatrixAdapter(cliqueMatrix);
            var newMatrix = adapter.Transform();

            _commands.Clear();
            _commandCounter = -1;

            //сборка графа из таблицы связности
            Builder builder = new GraphBuilder();
            _director = new Director(builder);
            _director.Construct(newMatrix);
            Command.Graph = builder.GetResult();

            //отрисовка графа
            _creator = new GraphPainter(Command.Graph);
            _creator.Draw();

            SpDeletedClicks.Children.Clear();
        }

        #endregion

        #region Воспомогательные функции

        /// <summary>
        /// Удаление повторяющихся ребер
        /// </summary>
        /// <param name="gr"></param>
        public void RemoveUnnecessaryEdges(BidirectionalGraph<DataVertex, DataEdge> gr)
        {
            var indexes = new List<int>();
            var edges = gr.Edges.ToList();

            foreach (var e in edges)
            {
                var index = edges.FindLastIndex(x => (x.Source.ID == e.Source.ID && x.Target.ID == e.Target.ID));
                if (index != -1)
                    indexes.Add(index);
            }

            indexes.Reverse();
            foreach (var i in indexes)
            {
                edges.RemoveAt(i);
            }

            gr.Edges.ToList().Clear();

            foreach (var e in edges)
            {
                gr.AddEdge(e);
            }
        }

        /// <summary>
        /// Путь к файлу с графом
        /// </summary>
        /// <returns></returns>
        public string GetNameOfFileWithGraph()
        {
            var dlg = new System.Windows.Forms.OpenFileDialog()
            {
                Filter = @"Text files(*.txt)|*.txt|All files(*.*)|*.*"
            };
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                return "";
            // получаем выбранный файл
            var filename = dlg.FileName;

            return filename;
        }

        /// <summary>
        /// Сконструировать таблицу связзности загруженного графа
        /// </summary>
        /// <param name="filename"></param>
        public int[,] ReadGraphFromFile(string filename)
        {
            var input = File.ReadAllText(filename).Replace("\r\n", "\n").Replace(" \n", "\n").Split('\n').ToList();
            var size = Convert.ToInt32(input[0]);
            input.RemoveAt(0);
            var i = 0;

            var result = new int[size, size];
            foreach (var row in input)
            {
                var j = 0;
                foreach (var col in row.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    result[i, j] = int.Parse(col.Trim());
                    j++;
                }
                i++;
            }
            return (int[,])result.Clone();
        }

        #endregion

        #region Перемещение вершин

        /// <summary>
        /// Сбросить режим перемещения вершин
        /// </summary>
        /// <param name="soft"></param>
        private void ClearSelectMode(bool soft = false)
        {
            GraphArea.VertexList.Values
                .Where(DragBehaviour.GetIsTagged)
                .ToList()
                .ForEach(a =>
                {
                    HighlightBehaviour.SetHighlighted(a, false);
                    DragBehaviour.SetIsTagged(a, false);
                });

            if (!soft)
                GraphArea.SetVerticesDrag(false);
        }

        /// <summary>
        /// Выбор вершины
        /// </summary>
        /// <param name="vc"></param>
        private static void SelectVertex(DependencyObject vc)
        {
            if (DragBehaviour.GetIsTagged(vc))
            {
                HighlightBehaviour.SetHighlighted(vc, false);
                DragBehaviour.SetIsTagged(vc, false);
            }
            else
            {
                HighlightBehaviour.SetHighlighted(vc, true);
                DragBehaviour.SetIsTagged(vc, true);
            }
        }

        private void GraphArea_VertexSelected(object sender, GraphX.Controls.Models.VertexSelectedEventArgs args)
        {
            if (args.MouseArgs.LeftButton == MouseButtonState.Pressed)
            {
                SelectVertex(args.VertexControl);
            }
        }

        private void BChangeMode_Checked(object sender, RoutedEventArgs e)
        {
            if (BChangeMode.IsChecked == true && Equals(sender, BChangeMode))
            {
                ZoomCtrl.Cursor = Cursors.Hand;
                GraphArea.SetVerticesDrag(true, true);
            }
        }

        private void BChangeMode_Unchecked(object sender, RoutedEventArgs e)
        {
            ClearSelectMode();
        }

        #endregion

        private void BAbout_Click(object sender, RoutedEventArgs e)
        {
            var wnd = new About();
            wnd.ShowDialog();
        }

        private void BHelp_Click(object sender, RoutedEventArgs e)
        {
            var wnd = new Help();
            wnd.ShowDialog();
        }
    }
}
