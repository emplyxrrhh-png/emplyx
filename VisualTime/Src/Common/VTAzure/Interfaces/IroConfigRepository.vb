Imports Robotics.Base.DTOs

Public Interface IroConfigRepository

    Function GetConfigParameter(eParameter As roConfigParameter) As roAzureConfig

    Function SaveConfigParameter(oParam As roAzureConfig) As Boolean

End Interface