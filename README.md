# Método Montante - Windows Forms

Aplicación de escritorio en C# (Windows Forms) que resuelve sistemas de ecuaciones lineales usando el **Método de Montante (Bareiss)**. Proporciona una interfaz interactiva donde el usuario puede ingresar ecuaciones en notación algebraica y obtener soluciones exactas paso a paso.

## 🚀 Características

- Ingreso dinámico de ecuaciones mediante `InputBox`.
- Validación de ecuaciones algebraicas lineales.
- Conversión automática a matriz aumentada.
- Aplicación del algoritmo de Montante con pivoteo.
- Visualización paso a paso en `DataGridView`.
- Cálculo y visualización de las soluciones finales.

## 🖥️ Requisitos

- Windows
- Visual Studio con soporte para Windows Forms
- .NET Framework (4.x recomendado)

## 📦 Cómo ejecutar

1. Clona el repositorio:

2. Abre el archivo .sln en Visual Studio.

3. Ejecuta el proyecto (F5 o botón "Start").

## ✍️ Uso

1. Al iniciar, presiona el botón para resolver el sistema.

2. Ingresa el número de ecuaciones.

3. Introduce las ecuaciones con formato, por ejemplo:
```
2x+3y=6
x-y=1
``` 

5. Se mostrará la matriz original, la matriz resultante, y las soluciones.

## ⚠️ Restricciones

- Solo ecuaciones lineales sin potencias ni funciones.

- Las variables deben estar escritas con una sola letra (x, y, z...).
