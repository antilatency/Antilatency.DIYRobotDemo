using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using System;

public static class FFT {
    private static int MAX = 0;

    private static int log2(int N) {
        int retVal = 0;
        while (N > 0) {
            N >>= 1;
            retVal++;
        }
        return retVal - 1;
    }

    public static Boolean isPowerOf2(int N) {
        Boolean retVal = false;

        if (N > 0 && (N & (N - 1)) == 0) {
            retVal = true;
        }


        return retVal;
    }

    private static int reverse(int N, int n) {
        int retVal = 0;
        int K = log2(N);

        for (int i = 1; i <= K; i++) {
            int x = n & (1 << (K - i));
            if (x > 0) {
                retVal |= 1 << (i - 1);
            }
        }

        return retVal;
    }

    private static Complex[] order(Complex[] vettore, int N) {
        Complex[] retVal = new Complex[MAX];
        for (int i = 0; i < N; i++) {
            retVal[i] = vettore[reverse(N, i)];
        }
        return retVal;
    }

    private static Complex w(int N) {
        double myReal = 1.0 * System.Math.Cos(2 * System.Math.PI / N);
        double myImag = 1.0 * System.Math.Sin(2 * System.Math.PI / N);

        Complex retval = new Complex(myReal, myImag);

        return retval;
    }

    private static Complex[] transform(Complex[] vettore, int N) {
        Complex[] retVal = order(vettore, N);
        Complex[] tmp = new Complex[MAX];
        int n = 1;
        for (int j = 0; j < log2(N); j++) {
            for (int i = 0; i < N; i++) {
                if ((i & n) == 0) {
                    tmp[i] = retVal[i];
                    retVal[i] = retVal[i] +
                      Complex.Pow(w(N), -i % n) * retVal[i + n];
                } else {
                    retVal[i] = tmp[i - 1] -
                      Complex.Pow(w(N), -i % n) * retVal[i];
                }
            }
            n *= 2;
        }

        return retVal;
    }

    public static Complex[] performFFT(Complex[] vettore, int N, double d) {
        MAX = N;
        Complex[] retVal = transform(vettore, N);
        for (int i = 0; i < N; i++) {
            retVal[i] *= d;
        }

        return retVal;
    }

    public static Complex[] performFFT(Complex[] vettore, int N) {
        MAX = N;
        Complex[] vector = transform(vettore, N);

        return vector;
    }
}