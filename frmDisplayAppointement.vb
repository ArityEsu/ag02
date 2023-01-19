Public Class frmDisplayAppointement

    Private listFlDay As New List(Of FlowLayoutPanel)
    Private currentDate As DateTime = DateTime.Today
    Dim obj As New Resizer

    Private Sub frmDisplayAppointement_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        GenerateDayPanel(42)
        displayCurrentDate()
        obj.FindAllControls(Me)
    End Sub


    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        obj.ResizeAllControls(Me)
    End Sub

    Private Sub addNewAppointment(ByVal sender As Object, e As EventArgs)
        Dim day As Integer = CType(sender, FlowLayoutPanel).Tag
        If day <> 0 Then
            With frmManageAppointements
                .AppID = 0
                .txtName.Text = ""
                .txtAddress.Text = ""
                .txtComment.Text = ""
                .dtpDate.Value = New Date(currentDate.Year, currentDate.Month, day)
                .ShowDialog()
            End With
            displayCurrentDate()
        End If

    End Sub

    Private Sub showAppointmentDetail(sender As Object, e As EventArgs)
        Dim appID As Integer = CType(sender, LinkLabel).Tag
        Dim sql As String = $"select * from appointment where ID = {appID}"
        Dim dt As DataTable = queryAsDataTable(sql)
        If dt.Rows.Count > 0 Then
            Dim row As DataRow = dt.Rows(0)
            With frmManageAppointements
                .AppID = appID
                .txtName.Text = row("ContactName")
                .txtAddress.Text = row("Address")
                .txtComment.Text = row("Comment")
                .dtpDate.Value = row("AppDate")
                .ShowDialog()
            End With
            displayCurrentDate()
        End If
    End Sub

    Private Sub addAppointmentsToF1Day(ByVal startDayAtF1Number As Integer)
        Dim startDate As DateTime = New Date(currentDate.Year, currentDate.Month, 1)
        Dim endDate As DateTime = startDate.AddMonths(1).AddDays(-1)

        'Dim sql As String = $"select * from appointment where AppDate between #{Format(startDate, "yyyy/MM/dd")}# and #{Format(endDate, "yyyy/MM/dd")}#"
        'Dim sql As String = "Select * From appointment Where AppDate Between #" & Format(startDate, "MM/yyyy") & "# And #" & Format(endDate, "MM/yyyy") & "#"
        'Dim sql As String = $"select * from appointment where AppDate between #{Format(startDate, "yyyy/MM/dd")}# and #{Format(endDate, "yyyy/MM/dd")}#"

        Dim sql As String = $"select * from appointment where AppDate between #{startDate.ToShortDateString()}# and #{endDate.ToShortDateString()}#"
        Dim dt As DataTable = queryAsDataTable(sql)

        For Each row As DataRow In dt.Rows
            Dim appDay As DateTime = DateTime.Parse(row("AppDate"))
            Dim link As New LinkLabel
            link.AutoSize = True
            '  link.MaximumSize(10, 20)
            link.Tag = row("ID")
            link.Name = $"link{row("ID")}"
            link.Text = row("ContactName")
            AddHandler link.Click, AddressOf showAppointmentDetail
            listFlDay((appDay.Day - 1) + (startDayAtF1Number - 1)).Controls.Add(link)
        Next
    End Sub


    Private Function getFirstDayofWeekOfCurrentDate() As Integer
        Dim firstDayOfMonth As DateTime = New Date(currentDate.Year, currentDate.Month, 7)
        ' Return firstDayOfMonth.DayOfWeek
        Return firstDayOfMonth.DayOfWeek + 1
    End Function

    Private Function getTotalDaysOfCurrentDate() As Integer
        Dim firstDayofCurrentDate As DateTime = New Date(currentDate.Year, currentDate.Month, 1)
        Return firstDayofCurrentDate.AddMonths(1).AddDays(-1).Day
    End Function

    Private Sub displayCurrentDate()
        lblMonthAndYear.Text = currentDate.ToString("MMMM,yyyy")
        Dim firstDayAtNumber As Integer = getFirstDayofWeekOfCurrentDate()
        Dim totalDay As Integer = getTotalDaysOfCurrentDate()
        AddLabe1DayToF1Day(firstDayAtNumber, totalDay)
        addAppointmentsToF1Day(firstDayAtNumber)
    End Sub

    Private Sub prevMonth()
        currentDate = currentDate.AddMonths(-1)
        displayCurrentDate()
    End Sub

    Private Sub nextMonth()
        currentDate = currentDate.AddMonths(1)
        displayCurrentDate()
    End Sub

    Private Sub today()
        currentDate = DateAndTime.Today
        displayCurrentDate()
    End Sub

    Private Sub GenerateDayPanel(ByVal totalDays As Integer)
        flDays.Controls.Clear()
        listFlDay.Clear()

        For i As Integer = 1 To totalDays
            Dim flowLayPan As New FlowLayoutPanel
            flowLayPan.Name = $"f1Day{i}"
            flowLayPan.Size = New Size(129, 105)
            flowLayPan.BackColor = Color.White
            flowLayPan.BorderStyle = BorderStyle.FixedSingle
            flowLayPan.Cursor = Cursors.Hand
            flowLayPan.AutoScroll = True
            AddHandler flowLayPan.Click, AddressOf addNewAppointment
            flDays.Controls.Add(flowLayPan)
            listFlDay.Add(flowLayPan)
        Next
    End Sub

    Private Sub AddLabe1DayToF1Day(ByVal startDayAtF1Number As Integer, ByVal totalDaysInMonth As Integer)
        For Each fl As FlowLayoutPanel In listFlDay
            fl.Controls.Clear()
            fl.Tag = 0
            fl.BackColor = Color.White
        Next

        For i As Integer = 1 To totalDaysInMonth
            Dim lbl As New Label
            lbl.Name = $"lblDay{i}"
            lbl.AutoSize = False
            lbl.TextAlign = ContentAlignment.MiddleRight
            lbl.Size = New Size(120, 23)
            lbl.Text = i
            lbl.Font = New Font("Micrsoft Sans Serif", 12)
            listFlDay((i - 1) + (startDayAtF1Number - 1)).Tag = i
            listFlDay((i - 1) + (startDayAtF1Number - 1)).Controls.Add(lbl)

            If New Date(currentDate.Year, currentDate.Month, i) = Date.Today Then
                listFlDay((i - 1) + (startDayAtF1Number - 1)).BackColor = Color.Aqua
            End If
        Next

    End Sub

    Private Sub btnPrevMonth_Click(sender As Object, e As EventArgs) Handles btnPrevMonth.Click
        prevMonth()
    End Sub

    Private Sub btnNextMonth_Click(sender As Object, e As EventArgs) Handles btnNextMonth.Click
        nextMonth()
    End Sub

    Private Sub btnToday_Click(sender As Object, e As EventArgs) Handles btnToday.Click
        today()
    End Sub

End Class


Public Class Resizer
    Private Structure ControlInfo
        Public name As String
        Public parentName As String
        Public leftOffsetPercent As Double
        Public topOffsetPercent As Double
        Public heightPercent As Double
        Public originalHeight As Integer
        Public originalWidth As Integer
        Public widthPercent As Double
        Public originalFontSize As Single
    End Structure


    Private ctrlDict As Dictionary(Of String, ControlInfo) = New Dictionary(Of String, ControlInfo)

    Public Sub FindAllControls(thisCtrl As Control)
        For Each ctl As Control In thisCtrl.Controls
            Try
                If Not IsNothing(ctl.Parent) Then
                    Dim parentHeight = ctl.Parent.Height
                    Dim parentWidth = ctl.Parent.Width

                    Dim c As New ControlInfo
                    c.name = ctl.Name
                    c.parentName = ctl.Parent.Name
                    c.topOffsetPercent = Convert.ToDouble(ctl.Top) / Convert.ToDouble(parentHeight)
                    c.leftOffsetPercent = Convert.ToDouble(ctl.Left) / Convert.ToDouble(parentWidth)
                    c.heightPercent = Convert.ToDouble(ctl.Height) / Convert.ToDouble(parentHeight)
                    c.widthPercent = Convert.ToDouble(ctl.Width) / Convert.ToDouble(parentWidth)
                    c.originalFontSize = ctl.Font.Size
                    c.originalHeight = ctl.Height
                    c.originalWidth = ctl.Width
                    ctrlDict.Add(c.name, c)
                End If

            Catch ex As Exception
                Debug.Print(ex.Message)
            End Try

            If ctl.Controls.Count > 0 Then
                FindAllControls(ctl)
            End If

        Next '-- For Each

    End Sub

    Public Sub ResizeAllControls(thisCtrl As Control)

        Dim fontRatioW As Single
        Dim fontRatioH As Single
        Dim fontRatio As Single
        Dim f As Font

        For Each ctl As Control In thisCtrl.Controls
            Try
                If Not IsNothing(ctl.Parent) Then
                    Dim parentHeight = ctl.Parent.Height
                    Dim parentWidth = ctl.Parent.Width

                    Dim c As New ControlInfo

                    Dim ret As Boolean = False
                    Try
                        '-- Get the current control's info from the control info dictionary
                        ret = ctrlDict.TryGetValue(ctl.Name, c)

                        '-- If found, adjust the current control based on control relative
                        '-- size and position information stored in the dictionary
                        If (ret) Then
                            '-- Size
                            ctl.Width = Int(parentWidth * c.widthPercent)
                            ctl.Height = Int(parentHeight * c.heightPercent)

                            '-- Position
                            ctl.Top = Int(parentHeight * c.topOffsetPercent)
                            ctl.Left = Int(parentWidth * c.leftOffsetPercent)

                            '-- Font
                            f = ctl.Font
                            fontRatioW = ctl.Width / c.originalWidth
                            fontRatioH = ctl.Height / c.originalHeight
                            fontRatio = (fontRatioW +
                            fontRatioH) / 2 '-- average change in control Height and Width
                            ctl.Font = New Font(f.FontFamily,
                            c.originalFontSize * fontRatio, f.Style)

                        End If
                    Catch
                    End Try
                End If
            Catch ex As Exception
            End Try

            '-- Recursive call for controls contained in the current control
            If ctl.Controls.Count > 0 Then
                ResizeAllControls(ctl)
            End If

        Next '-- For Each
    End Sub

End Class