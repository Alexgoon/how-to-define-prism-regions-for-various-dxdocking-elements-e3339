﻿Imports Microsoft.VisualBasic
Imports System.ComponentModel.Composition
Imports System.Text.RegularExpressions
Imports DevExpress.Xpf.Bars

Namespace PrismOnDXDocking.Infrastructure
	<Export(GetType(IMenuService))>
	Public Class MenuService
		Implements IMenuService

		Private ReadOnly manager As BarManager
		Private ReadOnly bar As Bar
		<ImportingConstructor>
		Public Sub New(ByVal shell As Shell)
			Me.manager = shell.BarManager
			Me.bar = shell.MainMenu
		End Sub
        Public Sub Add(ByVal item As MenuItem) Implements IMenuService.Add
            Dim parent As BarSubItem = GetParent(item.Parent)
            Dim button As BarButtonItem = New BarButtonItem With {.Content = item.Title, .Command = item.Command, .Name = "bbi" & Regex.Replace(item.Title, "[^a-zA-Z0-9]", "")}
            manager.Items.Add(button)
            parent.ItemLinks.Add(New BarButtonItemLink With {.BarItemName = button.Name})
        End Sub

        Private Function GetParent(ByVal parentName As String) As BarSubItem
			For Each item As BarItem In manager.Items
				Dim button As BarSubItem = TryCast(item, BarSubItem)
				If button IsNot Nothing AndAlso button.Content.ToString() = parentName Then
					Return button
				End If
			Next item
			Dim newParent As BarSubItem = New BarSubItem With {.Content = parentName, .Name = "bsi" & Regex.Replace(parentName, "[^a-zA-Z0-9]", "")}
			manager.Items.Add(newParent)
			bar.ItemLinks.Add(New BarSubItemLink With {.BarItemName = newParent.Name})
			Return newParent
		End Function
	End Class
End Namespace