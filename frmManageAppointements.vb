Public Class frmManageAppointements

    Public AppID As Integer = 0

    Private Sub frmManageAppointements_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        btnDelete.Visible = AppID <> 0

    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        If String.IsNullOrWhiteSpace(txtName.Text) Then
            MsgBox("Customer or contact name is required.")
            Return
        End If

        If IsConfirm("DO YOU WANT TO SAVE ?") Then
            If AppID = 0 Then
                Dim sql As String = $"insert into appointment(AppDate, ContactName, Address, Comment) 
                            values('{dtpDate.Value.ToShortDateString()}', '{txtName.Text}', '{txtAddress.Text}', '{txtComment.Text}')"
                If InsertUpdateDelete(sql) Then
                    MsgBox("Inserted")
                    Close()
                Else
                    MsgBox("Failed")
                End If
            Else
                Dim sql As String = $"update appointment set AppDate = '{dtpDate.Value.ToShortDateString()}', 
                                    ContactName = '{txtName.Text}', Address = '{txtAddress.Text}',
                                    Comment = '{txtComment.Text}'
                                    where ID = {AppID}"
                If InsertUpdateDelete(sql) Then
                    MsgBox("Updated")
                Else
                    MsgBox("Failed")
                End If

            End If

        End If

    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Close()
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        If IsConfirm("Do you want to delete this appointment ?") Then
            Dim sql As String = $"delete from appointment where ID = {AppID}"
            If InsertUpdateDelete(sql) Then
                MsgBox("Deleted")
                Close()
            Else
                MsgBox("Delete Failed")
            End If
        End If
    End Sub
End Class
