Imports Robotics
Imports Robotics.Base.DTOs

Public Class ChannelsHelper

    Public Sub New()
    End Sub

    Private Function hasRightsOnChannel(passportChannels As String(), idChannel As Integer) As Boolean

        For Each channelDesc As String In passportChannels
            Dim idConversations As String() = channelDesc.Split("#")
            If idChannel = CInt(idConversations(0)) Then
                Return True
                Exit For
            End If
        Next
        Return False

    End Function

    Private Function getConversationChannel(complaintChannel As String, passportChannels As String(), otherChannels As String(), idConversation As Integer) As Integer
        Dim channelInfo As String() = {}
        For Each channelDesc As String In passportChannels
            channelInfo = channelDesc.Split("#")
            For Each idConv As String In channelInfo(1).Split(",")
                If idConversation = CInt(idConv) Then
                    Return CInt(channelInfo(0))
                End If
            Next
        Next

        For Each channelDesc As String In otherChannels
            channelInfo = channelDesc.Split("#")
            For Each idConv As String In channelInfo(1).Split(",")
                If idConversation = CInt(idConv) Then
                    Return CInt(channelInfo(0))
                End If
            Next
        Next

        channelInfo = complaintChannel.Split("#")
        For Each idConv As String In channelInfo(1).Split(",")
            If idConversation = CInt(idConv) Then
                Return CInt(channelInfo(0))
            End If
        Next

        Return -1

    End Function

    'complaintChannelId format "idChannel#idConversation1,idconversation2,idconversation3..idconversationN"
    'passportChannels format {"idChannel1#idConversation1,idconversation2,idconversation3..idconversationN","idChannel2#idConversation1,idconversation2,idconversation3..idconversationN"}
    'otherChannels format {"idChannel1#idConversation1,idconversation2,idconversation3..idconversationN","idChannel2#idConversation1,idconversation2,idconversation3..idconversationN"}
    Public Function SetAvailableChannels(idPassport As Integer, complaintChannelId As String, passportChannels As String(), otherChannels As String(), hasComplaintPermission As Boolean)

        Robotics.Web.Base.API.Fakes.ShimChannelsServiceMethods.GetAllChannelsPageBase =
            Function(reference)
                Dim oLst As New Generic.List(Of roChannel)

                For Each idChannel As String In passportChannels
                    Dim idConversations As String() = idChannel.Split("#")
                    oLst.Add(New Base.DTOs.roChannel With {.Id = CInt(idConversations(0)), .CreatedBy = idPassport, .IsComplaintChannel = False})
                Next

                Return oLst.ToArray()
            End Function

        Robotics.Web.Base.API.Fakes.ShimChannelsServiceMethods.GetComplaintChannelPageBaseBoolean =
            Function(reference, audit)
                Dim idComplaintChannel As Integer = CInt(complaintChannelId.Split("#")(0))

                If idComplaintChannel > 0 Then
                    Return New roChannel() With {
                    .IsComplaintChannel = True
                    }
                Else
                    Return Nothing
                End If

            End Function

        Robotics.Web.Base.API.Fakes.ShimChannelsServiceMethods.GetChannelInt32PageBaseBoolean =
            Function(idChannel, reference, bAudit)
                Dim idComplaintChannel As Integer = CInt(complaintChannelId.Split("#")(0))

                If hasRightsOnChannel(passportChannels, idChannel) Then
                    Return New Base.DTOs.roChannel With {.Id = idChannel, .CreatedBy = idPassport, .IsComplaintChannel = False}
                Else
                    If (idComplaintChannel = idChannel AndAlso hasComplaintPermission) Then
                        Return New Base.DTOs.roChannel With {.Id = idChannel, .CreatedBy = 0, .IsComplaintChannel = True}
                    Else
                        Return New Base.DTOs.roChannel With {.Id = -1, .CreatedBy = 0, .IsComplaintChannel = False}
                    End If
                End If

            End Function

        Robotics.Web.Base.API.Fakes.ShimChannelsServiceMethods.CreateOrUpdateChannelPageBaseroChannelRefBoolean =
           Function(reference, ByRef oChannel, bAudit)
               If oChannel.IsComplaintChannel Then
                   Return False
               End If

               If hasRightsOnChannel(passportChannels, oChannel.Id) OrElse oChannel.Id = 0 Then
                   Return True
               Else
                   Return False
               End If
           End Function

        Robotics.Web.Base.API.Fakes.ShimChannelsServiceMethods.UpdateChannelStatusPageBaseroChannelRefBoolean =
           Function(reference, ByRef oChannel, bAudit)
               Dim idComplaintChannel As Integer = CInt(complaintChannelId.Split("#")(0))
               If hasRightsOnChannel(passportChannels, oChannel.Id) OrElse (oChannel.Id = idComplaintChannel AndAlso oChannel.IsComplaintChannel AndAlso hasComplaintPermission) Then
                   Return True
               Else
                   Return False
               End If
           End Function

        Robotics.Web.Base.API.Fakes.ShimChannelsServiceMethods.DeleteChannelroChannelPageBaseBoolean =
           Function(oChannel, reference, bAudit)
               If oChannel.IsComplaintChannel Then
                   Return False
               End If

               If hasRightsOnChannel(passportChannels, oChannel.Id) Then
                   Return True
               Else
                   Return False
               End If
           End Function

        Robotics.Web.Base.API.Fakes.ShimChannelsServiceMethods.GetConversationByIdInt32PageBaseInt32Boolean =
           Function(idConversation, reference, iduser, bAudit)
               Dim iChannel As Integer = getConversationChannel(complaintChannelId, passportChannels, otherChannels, idConversation)
               Dim idComplaintChannel As Integer = CInt(complaintChannelId.Split("#")(0))

               If hasRightsOnChannel(passportChannels, iChannel) Then

                   Return New roConversation() With {
                                    .Id = idConversation,
                                    .Channel = New Base.DTOs.roChannel With {.Id = iChannel, .CreatedBy = idPassport, .IsComplaintChannel = False}
                                    }
               Else
                   If iChannel = idComplaintChannel AndAlso hasComplaintPermission Then
                       Return New roConversation() With {
                                    .Id = idConversation,
                                    .Channel = New Base.DTOs.roChannel With {.Id = iChannel, .CreatedBy = idPassport, .IsComplaintChannel = True}
                                    }
                   Else
                       Return New roConversation() With {
                                    .Id = -1,
                                    .Channel = New Base.DTOs.roChannel With {.Id = -1, .CreatedBy = 0, .IsComplaintChannel = False}
                                    }
                   End If

               End If
           End Function

        Robotics.Web.Base.API.Fakes.ShimChannelsServiceMethods.ChangeConversationStatePageBaseroConversationRefBoolean =
            Function(reference, ByRef oConversation, bAudit)

                Dim idComplaintChannel As Integer = CInt(complaintChannelId.Split("#")(0))

                If hasRightsOnChannel(passportChannels, oConversation.Channel.Id) OrElse (oConversation.Channel.Id = idComplaintChannel AndAlso hasComplaintPermission) Then
                    Return True
                Else
                    Return False
                End If

            End Function

        Robotics.Web.Base.API.Fakes.ShimChannelsServiceMethods.CreateMessagePageBaseroMessageRefBoolean =
            Function(reference, ByRef oMessage, bAudit)

                Dim idComplaintChannel As Integer = CInt(complaintChannelId.Split("#")(0))

                If hasRightsOnChannel(passportChannels, oMessage.Conversation.Channel.Id) OrElse (oMessage.Conversation.Channel.Id = idComplaintChannel AndAlso hasComplaintPermission) Then
                    Return True
                Else
                    Return False
                End If

            End Function

    End Function

End Class