﻿using appLaboratorioMedico.Utilities;
using ProyectoV1.Entities;
using ProyectoV1.Utilities.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectoV1.UI
{
    public partial class FrmEmployees : Form
    {
        //Se crean las variables para la instancia del formulario
        public static bool HayInstancia = false;
        public static FrmEmployees InstanciaActiva = null;
        Utiles utiles = new Utiles();  //Se crea una instancia a utilitarios
        LeerDatos leerDatos = new LeerDatos();
        List<Employees> employeeList = new List<Employees>(); //Se crea una instancia a la clase clientes como tipo lista
        private ErrorProvider oErrorProvider = new ErrorProvider();
        public FrmEmployees()
        {
            InitializeComponent();
        }

        private void btnGoBack_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                bool resultado = employeeList.Exists(x => x.employee_id.ToString() == txtID.Text.ToString());//Validamos si existe en la lista
                if (resultado)
                {
                    MessageBox.Show("The ID  " + txtID.Text + " already exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (txtID.Text.ToString() == string.Empty)
                {//Si el campo txtCedula está vacio
                    errorProvider1.SetError(txtID, "Obligatory field"); //En caso de que estuviese vacío activa el control error provider
                    return; //regresa a pantalla
                }

                //Antes de grabar validamos los datos
                // Validar Cedula
                if (string.IsNullOrEmpty(FormatStrings.GetDNINoFormat(this.txtID.Text)))
                {
                    errorProvider1.SetError(this.txtID, "ID number incorrect");
                    return;
                }

                // Validar Correo
                if (!LeerDatos.Es_Email(this.txtEmail))
                {
                    errorProvider1.SetError(this.txtEmail, "Email information is incorrect");
                    return;
                }

                // Validar Nombre
                if (!LeerDatos.Es_Cadena(this.txtName))
                {
                    errorProvider1.SetError(this.txtName, "Name with the wrong format");
                    return;
                }



                grabarArchivo(); //Graba el archivo
                //Se muestra dato en el grid view
                this.dataGridView1.Rows.Add(txtID.Text, txtName.Text, txtEmail.Text, txtPhone.Text, datetimeHireDate.Text, txtSalary.Text, txtManagerID.Text); 
                this.FrmEmployees_Load(null, null);

                //Limpia los archivos
                // limpirDatos();
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
            }
        }

        private void grabarArchivo()
        {
            try
            {
                dataGridView1.DataSource = null;  //Limpiar el datagrid
                dataGridView1.Rows.Clear();//Limpiar el datagrid

                StreamWriter archivo = new StreamWriter("employee.txt", true); //Abre el archivo y agrega el append del mismo


                archivo.WriteLine(txtID.Text + ";" + txtName.Text.ToString().ToUpper() + ";" + txtEmail.Text.ToString().ToUpper() + ";" + txtPhone.Text.ToString().ToUpper() + ";" + datetimeHireDate.Text.ToString().ToUpper() + ";" + txtSalary.Text.ToString().ToUpper() + ";" + txtManagerID.Text.ToString().ToUpper());//Se agregan al registro
                archivo.Close(); //Cierra el archivo
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (!txtID.Text.Equals(""))
                {
                    bool resultado = employeeList.Exists(x => x.employee_id.ToString() == txtID.Text.ToString());//Validamos si existe en la lista
                    //Utilizando LinQ se consulta si existe el valor a borrar
                    if (resultado)
                    {
                        for (int i = 0; i < dataGridView1.Rows.Count; i++)//Se recorre el contenido de todas las filas del grid menos 1 dado a que el grid mantiene la linea de carga activa si se elimina la opción de agregar linea en el grid entonces se procede a quitar el menos 1
                        {
                            if (txtID.Text == dataGridView1.Rows[i].Cells[0].Value.ToString())
                            {//Si el dato de la cedula indicado es igual al valor que apunta la fila en la columna 0= Cedula
                                dataGridView1.Rows.RemoveAt(i); //Elimina la línea del grid que está apuntando
                                GrabarBorrado(); //Realiza proceso de eliminado del archivo
                                MessageBox.Show("Registration deleted sucessfully", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);//Muestra mensaje
                            }
                        }
                        //  this.FrmCustomers_Load(null, null);//Hacemos instancia del load
                    }
                }
                else
                {
                    errorProvider1.SetError(txtID, "Obligatory field");
                }
                limpirDatos();
            }
            catch (Exception er)
            {
                MessageBox.Show(utiles.mensajeCatch(er), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void limpirDatos()
        {
            txtID.Clear();
            txtName.Clear();
            txtEmail.Clear();
            txtPhone.Clear();
            txtSalary.Clear();
            txtManagerID.Clear();
            txtID.Focus(); //Acciona el cursor al control del textbox cedula
        }

        private void FrmEmployees_Load(object sender, EventArgs e)
        {
            txtID.Focus();  //Coloca el accionar del focus en el control textbox de la cedula
            HayInstancia = true; //Definición de variables para la instancia de carga de pantalla
            InstanciaActiva = this;
            employeeList.Clear(); //Limpiamos los datos
            errorProvider1.Clear();

            try
            {
                //Consulta si el archivo clientes.txt no existe 
                if (!File.Exists("employees.txt"))
                {
                    StreamWriter archivo = new StreamWriter("employees.txt");//En caso de no existir lo crea
                    archivo.Close();//Cierra el archivo
                }
                else
                {
                    StreamReader archivo = new StreamReader("employees.txt");//Hace lectura del archivo plano
                    while (!archivo.EndOfStream) //Recorre el archivo
                    {
                        String datos = archivo.ReadLine();//Recorre los registros (cedula, nombre,edad, correo electrónico)
                        String[] registros = datos.Split(';');//Se realiza un split de los datos obtenidos
                        Employees cli = new Employees();//Se crea instancia de clientes
                        cli.employee_id = int.Parse(registros[0].ToString());//Se asignan los valores a la instancia
                        cli.employee_name = registros[1];
                        cli.employee_email = registros[2];
                        cli.phone_number = registros[3];
                        cli.hire_date = DateTime.Parse(registros[4].ToString());
                        cli.salary = Decimal.Parse(registros[5].ToString());
                        cli.manager_id = int.Parse(registros[6].ToString());
                        
                        employeeList.Add(cli);//Se le agrega a la lista la instancia creada
                        //Formas de cargar un grid
                        //1- Por medio de agregacion
                        dataGridView1.Rows.Add(cli.employee_id, cli.employee_name, cli.employee_email, cli.phone_number, cli.hire_date,cli.salary, cli.manager_id);
                    }
                    archivo.Close();//Cierra el archivo
                    //2- Por medio del Dataset según una lista, para utilizar esta opcion se recomienda que no tenga columnas creadas
                    //var listaGrid = clientesList;
                    //dgvDatos.AutoGenerateColumns = true;
                    //dgvDatos.DataSource = listaGrid;
                }
                limpirDatos();
            }
            catch (Exception er)
            {
                MessageBox.Show(utiles.mensajeCatch(er), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GrabarBorrado()
        {
            try
            {
                dataGridView1.DataSource = null;  //Limpiar el datagrid
                dataGridView1.Rows.Clear();//Limpiar el datagrid

                File.WriteAllText("employees.txt", "");
                StreamWriter archivo = new StreamWriter("employees.txt", true);//Realiza lectura del archivo y permite agregar datos (append)


                //Utilizando LinQ
                List<Employees> lista = employeeList.Where(x => x.ToString() != txtID.Text).ToList();//Obtenemos la lista de todos los registros donde la cedula sea diferente a lo que tiene la cedula del campo textCedula
                foreach (var item in lista)//Se recorre la lista de datos
                {
                    archivo.WriteLine(item.employee_id.ToString() + ";" + item.employee_name.ToString().ToUpper() + ";" + item.employee_email.ToString().ToUpper() + ";" + item.phone_number.ToString().ToUpper() + item.hire_date.ToString().ToLower()  + item.salary.ToString().ToUpper() + "; " + item.manager_id.ToString().ToLower() + ";");//Se agregan al registro
                }
                archivo.Close();// Se cierra el registro
                employeeList.Clear();
            }
            catch (Exception er)
            {
                MessageBox.Show(utiles.mensajeCatch(er), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    

        private void FrmEmployees_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Se inactivan las variables creadas
            HayInstancia = false;
            InstanciaActiva = null;
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                for (int i = 0; i < dataGridView1.Rows.Count; i++) //Realiza recorrido de filas (validar si tiene el habilitar agregar filas para dejar o eliminar el -1)
                {
                    txtID.Text = this.dataGridView1.CurrentRow.Cells[0].Value.ToString();//Muestra los datos de la fila seleccionada y lo coloca en el campo Cedula
                    txtName.Text = this.dataGridView1.CurrentRow.Cells[1].Value.ToString().ToUpper();
                    txtEmail.Text = this.dataGridView1.CurrentRow.Cells[2].Value.ToString().ToUpper();
                    txtPhone.Text = (this.dataGridView1.CurrentRow.Cells[3].Value.ToString().ToLower());
                    datetimeHireDate.Text = (this.dataGridView1.CurrentRow.Cells[4].Value.ToString().ToLower());
                    txtSalary.Text = this.dataGridView1.CurrentRow.Cells[5].Value.ToString().ToUpper();
                    txtManagerID.Text = this.dataGridView1.CurrentRow.Cells[6].Value.ToString().ToUpper();
            
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(utiles.mensajeCatch(er), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExportToPDF_Click(object sender, EventArgs e)
        {
            ExportarGrid exportar = new ExportarGrid();
            exportar.ExportarPDF2(dataGridView1, "NewEmployee");
        }

        private void btnEmail_Click(object sender, EventArgs e)
        {
            try
            {
                //Si txtCedula es diferente de vacio envia correo
                if (!txtID.Equals(""))
                {

                    //Construye los datos en una tabla por medio de HTML
                    String mensaje = "";
                    //Se crea una tabla algunos datos importantes para tomar en cuenta
                    //<tr>  definir una fila | <th> define un encabezado | <td> define una celda </tr> Cierre de fila | </th> Cierre de Encabezado  | </td>  Cierre de Celda  | <br>  Salto
                    mensaje += "<table width=100% border=1 cellpadding=0 cellspacing=0>";
                    //Se define una fila con una columna que dice UTN Ingreso
                    mensaje += "<tr><td bgcolor=#004080 style='color:white; padding:4px;' width=40%><strong><div align=center>Magic Book Library</div></strong></td>";
                    //Se define una columna con estilo de color que indica mensaje
                    mensaje += "<td style='padding:4px;'>Welcome to Magic Book Library Team. </td></tr>";
                    mensaje += "</table><br>"; //Cierre de tabla y realiza un saldo

                    //Creación de otra tabla 
                    mensaje += "<table width=100% border=1 cellpadding=0 cellspacing=0>";
                    //Creacion de fila y se agrega un encabezado con los datos de ESTOS SON SUS DATOS
                    mensaje += "<tr><th colspan=6 style='padding:4px;'>This is your information</th></tr>";
                    //CREACION DE FILA NUEVO
                    mensaje += "<tr>";
                    //CREACION DE CELDA CON EL TITULO CEDULA    "ENCABEZADO"
                    mensaje += "<th bgcolor=#004080 style='color:white; padding:4px;'><div align=center>ID</div></th>";
                    //CREACION DE CELDA CON EL TIUTLO DE NOMBRE    "ENCABEZADO"
                    mensaje += "<th bgcolor=#004080 style='color:white; padding:4px;'><div align=center>NAME</div></th>";
                    //CREACION DE CELDA CON TITULO DE EDAD   "ENCABEZADO"
                    mensaje += "<th bgcolor=#004080 style='color:white; padding:4px;'><div align=center>EMAIL</div></th>";
                    //CREACION DE CELDA CON TITULO DE EDAD   "ENCABEZADO"
                    mensaje += "<th bgcolor=#004080 style='color:white; padding:4px;'><div align=center>PHONE</div></th>";
                    //CREACION DE CELDA CON TITULO DE EDAD   "ENCABEZADO"
                    mensaje += "<th bgcolor=#004080 style='color:white; padding:4px;'><div align=center>HIREDATE</div></th>";
                    //CREACION DE CELDA CON TITULO DE EDAD   "ENCABEZADO"
                    mensaje += "<th bgcolor=#004080 style='color:white; padding:4px;'><div align=center>SALARY</div></th>";
                    //CREACION DE CELDA CON TITULO DE EDAD   "ENCABEZADO"
                    mensaje += "<th bgcolor=#004080 style='color:white; padding:4px;'><div align=center>MANAGERID</div></th>";
                    //fIN DE FILA CREADA
                    mensaje += "</tr>";
                    //CRAECION DE NUEVA FILA
                    mensaje += "<tr>";
                    //SE AGREGAN LOS VALORES EN CELDAS NUEVAS CONSTRUYENDO EL STRING
                    mensaje += String.Format("<td style='padding:4px;'><div align=center>{0}</div></td>", txtID.Text);
                    mensaje += String.Format("<td style='padding:4px;'><div align=center>{0}</div></td>", txtName.Text);
                    mensaje += String.Format("<td style='padding:4px;'><div align=center>{0}</div></td>", txtEmail.Text);
                    mensaje += String.Format("<td style='padding:4px;'><div align=center>{0}</div></td>", txtPhone.Text);
                    mensaje += String.Format("<td style='padding:4px;'><div align=center>{0}</div></td>", datetimeHireDate.Text);
                    mensaje += String.Format("<td style='padding:4px;'><div align=center>{0}</div></td>", txtSalary.Text);
                    mensaje += String.Format("<td style='padding:4px;'><div align=center>{0}</div></td>", txtManagerID.Text);

                    mensaje += "</tr>";
                    //FIN DE CREACION DE TABLA
                    mensaje += "</table>";


                    String asunto = "Employee  included";
                    //String ruta = Path.GetFullPath(@"..\..\Instrucciones\Practica_Leccion02.pdf");
                    String ruta = "";
                    String adjunto = ruta;
                    String receptor = txtEmail.Text;
                    EnviarCorreo envioCorreo = new EnviarCorreo();
                    envioCorreo.enviarCorreoGmail(mensaje.ToString(), receptor, asunto, adjunto);

                }
                else
                {
                    MessageBox.Show("Email cannot be send", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(utiles.mensajeCatch(er), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}

