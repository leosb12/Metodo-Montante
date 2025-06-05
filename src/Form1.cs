using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WindowsFormsApplication13
{
    public partial class Form1 : Form
    {
        List<string> variables;

        public Form1()
        {
            InitializeComponent();
            variables = new List<string>();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
 
                int numEcuaciones = SolicitarNumeroEcuaciones();

                string[] ecuaciones = SolicitarEcuaciones(numEcuaciones);
           
                int[,] matriz = ConvertirSistemaAMatriz(ecuaciones, numEcuaciones);
                
                MostrarMatrizEnGrid(matriz, numEcuaciones, dgvMatriz);

                int[,] resultado = MetodoMontante(matriz);

                MostrarMatrizEnGrid(resultado, numEcuaciones, dgvMatrizResultado);

                MostrarSolucionesEnTextBox(resultado, numEcuaciones);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void MostrarMatrizEnGrid(int[,] matriz, int numEcuaciones, DataGridView dgv)
        {
            
            dgv.Columns.Clear();
            dgv.Rows.Clear();

         
            for (int j = 0; j < numEcuaciones + 1; j++)
            {
                string nombreColumna;
                if (j < numEcuaciones)
                    nombreColumna = variables[j];
                else
                    nombreColumna = "Término Independiente";

                dgv.Columns.Add("col" + j, nombreColumna);
            }

           
            for (int i = 0; i < numEcuaciones; i++)
            {
                object[] fila = new object[numEcuaciones + 1];
                for (int j = 0; j < numEcuaciones + 1; j++)
                {
                    fila[j] = matriz[i, j];
                }
                dgv.Rows.Add(fila);
            }
        }

        private void MostrarSolucionesEnTextBox(int[,] resultado, int numEcuaciones)
        {
            
            StringBuilder sb = new StringBuilder();

           
            sb.AppendLine("Soluciones:");

        
            for (int i = 0; i < numEcuaciones; i++)
            {
                // Calcular valor de la variable dividiendo término independiente entre coeficiente principal
                double valor = (double)resultado[i, numEcuaciones] / resultado[i, i];

              
                string nombreVariable = variables[i];

                
                sb.AppendLine($"{nombreVariable} = {valor}");
            }

            
            textBox1.Text = sb.ToString();
        }

        private int SolicitarNumeroEcuaciones()
        {
            
            int numEcuaciones = 0;

            
            while (numEcuaciones <= 0)
            {
                
                string input = Microsoft.VisualBasic.Interaction.InputBox(
                    "¿Cuántas ecuaciones tiene el sistema?", 
                    "Número de Ecuaciones",                 
                    "3"                                  
                );

                if (!int.TryParse(input, out numEcuaciones) || numEcuaciones <= 0)
                {
                  
                    MessageBox.Show("Por favor, ingresa un número válido de ecuaciones.");
                }
            }

     
            return numEcuaciones;
        }


        private string[] SolicitarEcuaciones(int numEcuaciones)
        {
            string[] ecuaciones = new string[numEcuaciones];

            for (int i = 0; i < numEcuaciones; i++)
            {
                bool ecuacionValida = false;

                while (!ecuacionValida)
                {
                    string input = Microsoft.VisualBasic.Interaction.InputBox(
                        $"Ingresa la ecuación {i + 1} (ej: 2x+3y-z=5):",
                        "Ecuación del Sistema",
                        "2x+3y-z=5"
                    );

                    string ecuacionSinEspacios = input.Replace(" ", "");

                    try
                    {
                        // Validaciones
                        if (ecuacionSinEspacios.Contains("^"))
                            throw new Exception("La ecuación contiene potencias. Solo se permiten ecuaciones lineales.");

                        if (Regex.IsMatch(ecuacionSinEspacios, @"[a-zA-Z]{2,}"))
                            throw new Exception("La ecuación contiene productos entre variables. Solo se permiten variables individuales.");

                        if (Regex.IsMatch(ecuacionSinEspacios.ToLower(), @"(sin|cos|tan|log|exp|sqrt)"))
                            throw new Exception("La ecuación contiene funciones no permitidas. Solo se permiten ecuaciones lineales.");

                        if (Regex.IsMatch(ecuacionSinEspacios, @"[()√π]"))
                            throw new Exception("La ecuación contiene símbolos inválidos. Solo se permiten sumas, restas, variables y números.");

                        if (!ecuacionSinEspacios.Contains("="))
                            throw new Exception("La ecuación debe tener el signo '='.");

                        string[] partes = ecuacionSinEspacios.Split('=');
                        if (partes.Length != 2 || string.IsNullOrWhiteSpace(partes[0]) || string.IsNullOrWhiteSpace(partes[1]))
                            throw new Exception("La ecuación no tiene el formato correcto. Debe ser algo como '2x+3y-z=5'.");

                        ecuaciones[i] = ecuacionSinEspacios;
                        ecuacionValida = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error en la ecuación {i + 1}: {ex.Message}");
                    }
                }
            }

            return ecuaciones;
        }


        private int[,] ConvertirSistemaAMatriz(string[] ecuaciones, int numEcuaciones)
        {
            int[,] matriz = new int[numEcuaciones, numEcuaciones + 1];
            variables = new List<string>();

            // Extraer variables desde la primera ecuación
            string firstEquation = ecuaciones[0].Replace(" ", "");
            MatchCollection varMatches = Regex.Matches(firstEquation, @"[a-zA-Z]");

            foreach (Match match in varMatches)
            {
                if (!variables.Contains(match.Value))
                    variables.Add(match.Value);
            }

            if (variables.Count != numEcuaciones)
                throw new Exception("El número de variables no coincide con el número de ecuaciones.");

            for (int i = 0; i < numEcuaciones; i++)
            {
                string ecuacion = ecuaciones[i].Replace(" ", "");
                string[] partes = ecuacion.Split('=');

                if (partes.Length != 2)
                    throw new Exception($"La ecuación {i + 1} no tiene el formato esperado.");

                string lhs = partes[0];
                string rhs = partes[1];

               
                int termIndependiente = int.Parse(rhs);

                int[] coeficientes = new int[numEcuaciones];

      
                foreach (string variable in variables)
                {
                    Match match = Regex.Match(lhs, $@"([+-]?)(\d*)({variable})");

                    int indexVar = variables.IndexOf(variable);

                    if (match.Success)
                    {
                        string signStr = match.Groups[1].Value;
                        string coefStr = match.Groups[2].Value;

                        int coeficiente = string.IsNullOrEmpty(coefStr) ? 1 : int.Parse(coefStr);

                        if (signStr == "-")
                            coeficiente *= -1;
                        else if (signStr == "+")
                            coeficiente *= 1;
                        else if (string.IsNullOrEmpty(signStr))
                            coeficiente *= 1;

                        coeficientes[indexVar] = coeficiente;

                      
                        lhs = lhs.Replace(match.Value, "");
                    }
                    else
                    {
        
                        coeficientes[indexVar] = 0;
                    }
                }

        
                MatchCollection remainingTerms = Regex.Matches(lhs, @"([+-])(\d+)");

                foreach (Match term in remainingTerms)
                {
                    string signStr = term.Groups[1].Value;
                    string coefStr = term.Groups[2].Value;

                    int value = int.Parse(coefStr);

                    if (signStr == "+")
                        termIndependiente -= value;
                    else if (signStr == "-")
                        termIndependiente += value;

                    lhs = lhs.Replace(term.Value, "");
                }

   
                if (!string.IsNullOrEmpty(lhs))
                    throw new Exception($"Formato inválido en la ecuación {i + 1}.");

                for (int j = 0; j < numEcuaciones; j++)
                {
                    matriz[i, j] = coeficientes[j];
                }
                matriz[i, numEcuaciones] = termIndependiente;
            }

            return matriz;
        }

        private int[,] MetodoMontante(int[,] matriz)
        {
            int n = matriz.GetLength(0);
            int[,] m = (int[,])matriz.Clone(); // Copiar matriz original
            int pivoteAnterior = 1;

            for (int k = 0; k < n; k++)
            {
                // Si el pivote actual es cero, intentar intercambiar filas
                if (m[k, k] == 0)
                {
                    bool pivotFound = false;

                    for (int i = k + 1; i < n; i++)
                    {
                        if (m[i, k] != 0)
                        {
                            for (int j = 0; j < n + 1; j++)
                            {
                                int temp = m[k, j];
                                m[k, j] = m[i, j];
                                m[i, j] = temp;
                            }
                            pivotFound = true;
                            break;
                        }
                    }

                    if (!pivotFound)
                        throw new Exception("El sistema no tiene solución única o es inconsistente (pivote cero sin posibilidad de intercambio).");
                }

                int pivote = m[k, k];

                // Aplicar fórmula de Montante a toda la fila
                for (int i = 0; i < n; i++)
                {
                    if (i != k)
                    {
                        for (int j = 0; j < n + 1; j++)
                        {
                            if (j != k)
                            {
                                m[i, j] = ((m[k, k] * m[i, j]) - (m[i, k] * m[k, j])) / pivoteAnterior;
                            }
                        }

                        m[i, k] = 0;
                    }
                }

                pivoteAnterior = pivote;
            }

            return m;
        }



        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

    

        