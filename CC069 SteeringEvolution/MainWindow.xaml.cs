using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SteeringEvolution
{
    public partial class MainWindow : Window
    {
        private Random Rnd = new Random();
        private Log my_Log = new Log(true, false);
        private bool logging;
        private List<Vector> food;
        private int foodCount = 200;
        private int foodValue = 10;
        private List<Vector> poison;
        private int poisonCount = 60;
        private int poisonValue = 50;
        private List<Agent> agents;
        private int PopulationNr = 0;
        private int agentCount = 15;
        private double Maxspeed = 1.5;
        private double MaxForce = 3.0;
        private double MutationRate = 30.0;
        private double stepSize = 0.1;
        private int maxAge = 0;
        private int eldestIndex = -1;
        private Agent best;
        private bool Rendering = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            logging = false;
            if (logging)
            {
                my_Log.AddItem("Start settings:");
                my_Log.AddItem("foodCount = " + foodCount.ToString());
                my_Log.AddItem("poisonCount = " + poisonCount.ToString());
                my_Log.AddItem("AgentCount = " + agentCount.ToString());
            }
            food = new List<Vector>();
            for (int I = 0; I < foodCount; I++)
            {
                food.Add(new Vector(canvas1.ActualWidth * (0.9 * Rnd.NextDouble() + 0.05), canvas1.ActualHeight * (0.9 * Rnd.NextDouble() + 0.05)));
            }
            poison = new List<Vector>();
            for (int I = 0; I < poisonCount; I++)
            {
                poison.Add(new Vector(canvas1.ActualWidth * (0.9 * Rnd.NextDouble() + 0.05), canvas1.ActualHeight * (0.9 * Rnd.NextDouble() + 0.05)));
            }
            agents = new List<Agent>();
            PopulationNr = 1;
            if (logging) my_Log.AddItem("Population Nr: " + PopulationNr.ToString());
            for (int I = 0; I < agentCount; I++)
            {
                agents.Add(new Agent(canvas1.ActualWidth * (0.9 * Rnd.NextDouble() + 0.05), canvas1.ActualHeight * (0.9 * Rnd.NextDouble() + 0.05), 1.0, Maxspeed, MaxForce));
                if (logging) my_Log.AddItem("Agent " + I.ToString() + " = " + agents[I].Status());
            }
            best = agents[0];
            CompositionTarget.Rendering += CompositionTarget_Rendering;
            Rendering = true;
        }

        private void CompositionTarget_Rendering(Object sender, EventArgs e)
        {
            if (!Rendering) return;
            int alive = 0;
            Agent newAgent;
            //Add some food
            if (Rnd.NextDouble() < 0.09)
            {
                food.Add(new Vector(canvas1.ActualWidth * (0.9 * Rnd.NextDouble() + 0.05), canvas1.ActualHeight * (0.9 * Rnd.NextDouble() + 0.05)));
            }
            //Add some poison
            if (poison.Count < 75)
            {
                if (Rnd.NextDouble() < 0.01)
                {
                    poison.Add(new Vector(canvas1.ActualWidth * (0.9 * Rnd.NextDouble() + 0.05), canvas1.ActualHeight * (0.9 * Rnd.NextDouble() + 0.05)));
                }
            }
            //Update the agents
            double actualhealth = 0.0;
            for (int I = agents.Count - 1; I >= 0; I--)
            {
                actualhealth = agents[I].Health;
                if (actualhealth > 0)
                {
                    alive += 1;
                    agents[I].Update(food, poison, foodValue, poisonValue);
                    if (agents[I].Health <= 0)
                    {
                        if (logging) my_Log.AddItem("Agent " + I.ToString() + "  died at age " + agents[I].Age.ToString());
                    }
                }
            }
            //Allow procreation
            for (int I = 0; I < agents.Count; I++)
            {
                if (agents[I].Health >= 200)
                {
                    newAgent = new Agent(agents[I]);
                    newAgent.Mutate(MutationRate, stepSize);
                    agents[I].Health = 100;
                    agents.Add(newAgent);
                    if (logging)
                    {
                        my_Log.AddItem("Agent " + I.ToString() + "  spawned a new agent:");
                        my_Log.AddItem("Parent = " + I.ToString() + agents[I].Status());
                        my_Log.AddItem("Child = " + (agents.Count - 1).ToString() + newAgent.Status());
                    }
                }
            }
            //Restart if all Agents die
            if (alive == 0)
            {
                if (logging)
                {
                    my_Log.AddItem("EXTICTION.");
                    my_Log.AddItem("Best Agent was " + best.Status());
                }
                PopulationNr += 1;
                if (logging) my_Log.AddItem("Making new Population (Nr: " + PopulationNr.ToString() + ")");
                food.Clear();
                for (int I = 0; I < foodCount; I++)
                {
                    food.Add(new Vector(canvas1.ActualWidth * (0.9 * Rnd.NextDouble() + 0.05), canvas1.ActualHeight * (0.9 * Rnd.NextDouble() + 0.05)));
                }
                poison.Clear();
                for (int I = 0; I < poisonCount; I++)
                {
                    poison.Add(new Vector(canvas1.ActualWidth * (0.9 * Rnd.NextDouble() + 0.05), canvas1.ActualHeight * (0.9 * Rnd.NextDouble() + 0.05)));
                }
                maxAge = 0;
                eldestIndex = -1;
                agents.Clear();
                for (int I = 0; I < agentCount; I++)
                {
                    if (100 * Rnd.NextDouble() > 100 - MutationRate)
                    {
                        newAgent = new Agent(best);
                        newAgent.Mutate(MutationRate, stepSize);
                        if (logging) my_Log.AddItem("Agent " + I.ToString() + " = Mutant of Best : " + newAgent.Status());
                    }
                    else
                    {
                        newAgent = new Agent(canvas1.ActualWidth * (0.9 * Rnd.NextDouble() + 0.05), canvas1.ActualHeight * (0.9 * Rnd.NextDouble() + 0.05), 1.0, Maxspeed, MaxForce);
                        if (logging) my_Log.AddItem("Agent " + I.ToString() + " = Random : " + newAgent.Status());
                    }
                    agents.Add(newAgent);
                }
            }
            //Show the eldest Agent status info
            for (int I = 0; I < agents.Count; I++)
            {
                if (agents[I].Health > 0 & agents[I].Age > maxAge)
                {
                    maxAge = agents[I].Age;
                    eldestIndex = I;
                    best = agents[I];
                }
            }
            if (eldestIndex >= 0)
            {
                TxtEldest.Text = "Age = " + best.Age.ToString() +
                "  ( Food = " + best.DNA[0].ToString("F2") +
                " ; Range = " + best.DNA[2].ToString("F2") +
                " )  ( Poison = " + best.DNA[1].ToString("F2") +
                " ; Range = " + best.DNA[3].ToString("F2") + " )";
            }
            TxtFoodCount.Text = food.Count.ToString();
            TxtPoisonCount.Text = poison.Count.ToString();
            Draw();
        }

        private void Draw()
        {
            canvas1.Children.Clear();
            Ellipse El;
            for (int I = 0; I < food.Count; I++)
            {
                El = new Ellipse()
                {
                    Width = 4,
                    Height = 4,
                    Stroke = Brushes.Green,
                    StrokeThickness = 1,
                    Fill = Brushes.Green
                };
                El.SetValue(Canvas.LeftProperty, food[I].X - 2);
                El.SetValue(Canvas.TopProperty, food[I].Y - 2);
                canvas1.Children.Add(El);
            }
            for (int I = 0; I < poison.Count; I++)
            {
                El = new Ellipse()
                {
                    Width = 4,
                    Height = 4,
                    Stroke = Brushes.Red,
                    StrokeThickness = 1,
                    Fill = Brushes.Red
                };
                El.SetValue(Canvas.LeftProperty, poison[I].X - 2);
                El.SetValue(Canvas.TopProperty, poison[I].Y - 2);
                canvas1.Children.Add(El);
            }
            for (int I = 0; I < agents.Count; I++)
            {
                if (agents[I].Health > 0) agents[I].Draw(canvas1);
            }
        }

        private void CbLogging_Click(object sender, RoutedEventArgs e)
        {
            logging = CbLogging.IsChecked.Value;
            if (!logging) my_Log.Flush();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            my_Log.StopLog();
        }
    }
}
