using System;
using System.Collections.Generic;
using System.Windows;

namespace Fourier_Complex_Freehand
{
    class DFT
    {
        public static List<Vector> Process(List<double> signal)
        {
            int N = signal.Count;
            List<Vector> result = new List<Vector>();
            double Re;
            double Im;
            double alfa;
            for (int K = 0; K < signal.Count; K++)
            {
                Re = 0.0;
                Im = 0.0;
                for (int I = 0; I < N; I++)
                {
                    alfa = 2 * Math.PI * K * I / N;
                    Re += signal[I] * Math.Cos(alfa);
                    Im += -1 * signal[I] * Math.Sin(alfa);
                }
                result.Add(new Vector(Re / N, Im / N));
            }
            return result;
        }

        public static List<Vector> Process(List<Point> signal)
        {
            int N = signal.Count;
            List<Vector> result = new List<Vector>();
            double Re;
            double Im;
            double a;
            double b;
            double c;
            double d;
            double alfa;
            for (int K = 0; K < signal.Count; K++)
            {
                Re = 0.0;
                Im = 0.0;
                for (int I = 0; I < N; I++)
                {
                    alfa = 2 * Math.PI * K * I / N;
                    a = signal[I].X;
                    b = signal[I].Y;
                    c = Math.Cos(alfa);
                    d = -1 * Math.Sin(alfa);
                    //(a+bi)*(c+di) = (ac-bd) + (ad+bc)i;
                    Re = Re + (a * c - b * d);
                    Im = Im + (a * d + b * c);
                }
                result.Add(new Vector(Re / N, Im / N));
            }
            return result;
        }

        public static List<Epicycle> GetEpicycles(List<double> signal)
        {
            int N = signal.Count;
            List<Epicycle> result = new List<Epicycle>();
            double Re;
            double Im;
            double alfa;
            for (int K = 0; K < signal.Count; K++)
            {
                Re = 0.0;
                Im = 0.0;
                for (int I = 0; I < N; I++)
                {
                    alfa = 2 * Math.PI * K * I / N;
                    Re += signal[I] * Math.Cos(alfa);
                    Im += -1 * signal[I] * Math.Sin(alfa);
                }
                result.Add(new Epicycle(K, new Vector(Re / N, Im / N)));
            }
            return result;
        }

        public static List<Epicycle> GetEpicycles(List<Point> signal)
        {
            int N = signal.Count;
            List<Epicycle> result = new List<Epicycle>();
            double Re;
            double Im;
            double a;
            double b;
            double c;
            double d;
            double alfa;
            for (int K = 0; K < signal.Count; K++)
            {
                Re = 0.0;
                Im = 0.0;
                for (int I = 0; I < N; I++)
                {
                    alfa = 2 * Math.PI * K * I / N;
                    a = signal[I].X;
                    b = signal[I].Y;
                    c = Math.Cos(alfa);
                    d = -1 * Math.Sin(alfa);
                    //(a+bi)*(c+di) = (ac-bd) + (ad+bc)i;
                    Re = Re + (a * c - b * d);
                    Im = Im + (a * d + b * c);
                }
                result.Add(new Epicycle(K, new Vector(Re / N, Im / N)));
            }
            return result;
        }

        public static List<Epicycle> GetSortedEpicycles(List<double> signal, bool ascendingOrder)
        {
            List<Epicycle> result = GetEpicycles(signal);
            SortEpicycles(result, ascendingOrder);
            return result;
        }

        public static List<Epicycle> GetSortedEpicycles(List<Point> signal, bool ascendingOrder)
        {
            List<Epicycle> result = GetEpicycles(signal);
            SortEpicycles(result, ascendingOrder);
            return result;
        }

        private static void SortEpicycles(List<Epicycle> epi, bool ascendingOrder)
        {
            double dummyFreq;
            double dummyAmp;
            double dummyPhase;
            for (int I = 0; I < epi.Count; I++)
            {
                for (int J = I + 1; J < epi.Count; J++)
                {
                    if (ascendingOrder)
                    {
                        if (epi[J].Amplitude < epi[I].Amplitude)
                        {
                            dummyFreq = epi[I].Freqency;
                            dummyAmp = epi[I].Amplitude;
                            dummyPhase = epi[I].Phase;
                            epi[I].Freqency = epi[J].Freqency;
                            epi[I].Amplitude = epi[J].Amplitude;
                            epi[I].Phase = epi[J].Phase;
                            epi[J].Freqency = dummyFreq;
                            epi[J].Amplitude = dummyAmp;
                            epi[J].Phase = dummyPhase;
                        }
                    }
                    else
                    {
                        if (epi[J].Amplitude > epi[I].Amplitude)
                        {
                            dummyFreq = epi[I].Freqency;
                            dummyAmp = epi[I].Amplitude;
                            dummyPhase = epi[I].Phase;
                            epi[I].Freqency = epi[J].Freqency;
                            epi[I].Amplitude = epi[J].Amplitude;
                            epi[I].Phase = epi[J].Phase;
                            epi[J].Freqency = dummyFreq;
                            epi[J].Amplitude = dummyAmp;
                            epi[J].Phase = dummyPhase;
                        }
                    }
                }

            }
        }
    }

//==================================================================================================================

    public class Epicycle
    {
        public double Freqency;
        public double Amplitude;
        public double Phase;

        public Epicycle(double Freq, Vector V)
        {
            Freqency = Freq;
            Amplitude = V.Length;
            Phase = Math.Atan2(V.Y, V.X);
        }
    }
}
