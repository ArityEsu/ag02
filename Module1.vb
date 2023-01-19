Imports System.Data.OleDb

Module Module1
    Private cn As New OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=D:\Bmap2\appointementSample\bin\Debug\database.accdb")

    Public Sub Connect()
        If cn.State = ConnectionState.Closed Then
            cn.Open()
        End If
    End Sub

    Public Function InsertUpdateDelete(ByVal sql As String) As Boolean
        Connect()
        Dim cmd As New OleDbCommand(sql, cn)
        Return cmd.ExecuteNonQuery() > 0
    End Function


    Public Function IsConfirm(ByVal message As String) As Boolean
        Return MessageBox.Show(message, "Confirm ?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes
    End Function


    Public Function queryAsDataTable(ByVal sql As String) As DataTable
        Dim da As New OleDbDataAdapter(sql, cn)
        Dim ds As New DataSet
        da.Fill(ds, "result")
        Return ds.Tables("result")
    End Function
End Module
