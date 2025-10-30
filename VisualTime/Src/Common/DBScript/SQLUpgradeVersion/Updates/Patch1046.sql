-- No borréis esta línea
DELETE TMPDummyRulesGroups
GO

INSERT INTO TMPDummyRulesGroups (DummyData) VALUES (
'[
  {
    "Id": 1001,
    "Name": "Grupo de Reglas de Justificación",
    "EditionStatus": 0,
    "Rules": [
      {
        "Id": 1,
        "Name": "Justificación por Enfermedad",
        "Description": "Automatiza la justificación de ausencias por enfermedad con certificado médico",
        "Type": 2,
        "TypeDescription": "Justification",
        "GroupId": 1001,
        "Tags": ["enfermedad", "justificación", "automática"],
        "EditionStatus": 0,
        "RuleDefinitions": [
          {
            "Id": 101,
            "Description": "Configuración inicial",
            "IdRule": 1,
            "EditionStatus": 0,
            "EmployeeContext": {
              "UserFields": "",
              "ComposeFilter": "All",
              "ComposeMode": "OR",
              "Filters": "",
              "Operation": "INCLUDE"
            },
            "Shifts": [
              {
                "IdShift": 42,
                "IdShiftGroup": 0,
                "IdRule": 1,
                "EditionStatus": 0,                
                "Order": 1
              },
              {
                "IdShift": 43,
                "IdShiftGroup": 0,
                "IdRule": 1,
                "EditionStatus": 0,  
                "Order": 2
              },
              {
                "IdShift": 41,
                "IdShiftGroup": 1,
                "IdRule": 1,
                "EditionStatus": 0,  
                "Order": 3
              }
            ],
            "Conditions": [],
            "Actions": [],
            "XmlDefinition": "",
            "ModifiedBy": "admin",
            "ModifiedDate": "2025-01-15T10:30:00",
            "EffectiveFrom": "2025-01-15T00:00:00",
            "EffectiveUntil": "2025-03-14T23:59:59"
          },
          {
            "Id": 102,
            "Description": "Actualización para incluir certificado digital",
            "IdRule": 1,
            "EditionStatus": 0,
            "EmployeeContext": {
              "UserFields": "",
              "ComposeFilter": "All",
              "ComposeMode": "OR",
              "Filters": "",
              "Operation": "INCLUDE"
            },
            "Shifts": [
              {
                "IdShift": 42,
                "IdShiftGroup": 0,
                "EditionStatus": 0,                                
                "IdRule": 1,
                "Order": 1
              },
              {
                "IdShift": 43,
                "IdShiftGroup": 0,
                "IdRule": 1,
                "EditionStatus": 0,  
                "Order": 2
              },
              {
                "IdShift": 41,
                "IdShiftGroup": 1,
                "IdRule": 1,
                "EditionStatus": 0,  
                "Order": 3
              }
            ],
            "Conditions": [],
            "Actions": [],
            "XmlDefinition": "",
            "ModifiedBy": "admin",
            "ModifiedDate": "2025-03-15T09:45:00",
            "EffectiveFrom": "2025-03-15T00:00:00",
            "EffectiveUntil": null
          }
        ]
      },
      {
        "Id": 2,
        "Name": "Justificación por Asuntos Personales",
        "Description": "Justificación de ausencias por motivos personales",
        "Type": 2,
        "TypeDescription": "Justification",
        "GroupId": 1001,
        "Tags": ["personal", "justificación", "automática"],
        "EditionStatus": 0,        
        "RuleDefinitions": [
          {
            "Id": 103,
            "Description": "Configuración inicial",
            "IdRule": 2,
            "EditionStatus": 0,
            "EmployeeContext": {
              "UserFields": "Antiguedad>1",
              "ComposeFilter": "Seniority",
              "ComposeMode": "AND",
              "Filters": ">1",
              "Operation": "INCLUDE"
            },
            "Shifts": [
              {
                "IdShift": 42,
                "IdShiftGroup": 0,
                "EditionStatus": 0,
                "IdRule": 2,
                "Order": 1
              },
              {
                "IdShift": 43,
                "IdShiftGroup": 0,
                "EditionStatus": 0,
                "IdRule": 2,
                "Order": 2
              }
            ],
            "Conditions": [],
            "Actions": [],
            "XmlDefinition": "",
            "ModifiedBy": "supervisor",
            "ModifiedDate": "2025-01-20T09:15:00",
            "EffectiveFrom": "2025-02-01T00:00:00",
            "EffectiveUntil": "2025-04-30T23:59:59"
          },
          {
            "Id": 104,
            "Description": "Actualización límite de días personales",
            "IdRule": 2,
            "EditionStatus": 0,
            "EmployeeContext": {
              "UserFields": "Antiguedad>1",
              "ComposeFilter": "Seniority",
              "ComposeMode": "AND",
              "Filters": ">1",
              "Operation": "INCLUDE"
            },
            "Shifts": [
              {
                "IdShift": 42,
                "IdShiftGroup": 0,
                "IdRule": 2,
                "EditionStatus": 0,
                "Order": 1
              },
              {
                "IdShift": 43,
                "IdShiftGroup": 0,
                "EditionStatus": 0,
                "IdRule": 2,
                "Order": 2
              }
            ],
            "Conditions": [],
            "Actions": [],
            "XmlDefinition": "",
            "ModifiedBy": "hr_manager",
            "ModifiedDate": "2025-05-01T08:30:00",
            "EffectiveFrom": "2025-05-01T00:00:00",
            "EffectiveUntil": null
          }
        ]
      }
    ]
  },
  {
    "Id": 1002,
    "Name": "Grupo de Reglas Diarias",
    "EditionStatus": 0,    
    "Rules": [
      {
        "Id": 3,
        "Name": "Control de Horas Extras",
        "Description": "Regla diaria para control y autorización de horas extras",
        "Type": 3,
        "TypeDescription": "Daily",
        "GroupId": 1002,
        "Tags": ["horas extras", "control", "diaria"],
        "EditionStatus": 0,        
        "RuleDefinitions": [
          {
            "Id": 105,
            "Description": "Configuración inicial",
            "IdRule": 3,
            "EmployeeContext": {
              "UserFields": "Departamento=Producción",
              "ComposeFilter": "Department",
              "ComposeMode": "AND",
              "Filters": "Producción",
              "Operation": "INCLUDE"
            },
            "Shifts": [
              {
                "IdShift": 42,
                "IdShiftGroup": 0,
                "IdRule": 3,
                "EditionStatus": 0,
                "Order": 1
              },
              {
                "IdShift": 63,
                "IdShiftGroup": 2,
                "EditionStatus": 0,
                "IdRule": 3,
                "Order": 2
              }
            ],
            "Conditions": [],
            "Actions": [],
            "XmlDefinition": "",
            "ModifiedBy": "manager",
            "ModifiedDate": "2025-02-10T14:30:00",
            "EffectiveFrom": "2025-03-01T00:00:00",
            "EffectiveUntil": "2025-06-30T23:59:59"
          },
          {
            "Id": 106,
            "Description": "Actualización para límite de horas extras",
            "IdRule": 3,
            "EditionStatus": 0,
            "EmployeeContext": {
              "UserFields": "Departamento=Producción",
              "ComposeFilter": "Department",
              "ComposeMode": "AND",
              "Filters": "Producción",
              "Operation": "INCLUDE"
            },
            "Shifts": [
              {
                "IdShift": 42,
                "IdShiftGroup": 0,
                "IdRule": 3,
                "EditionStatus": 0,
                "Order": 1
              },
              {
                "IdShift": 63,
                "IdShiftGroup": 2,
                "EditionStatus": 0,
                "IdRule": 3,
                "Order": 2
              }
            ],
            "Conditions": [],
            "Actions": [],
            "XmlDefinition": "",
            "ModifiedBy": "hr_director",
            "ModifiedDate": "2025-06-25T16:20:00",
            "EffectiveFrom": "2025-07-01T00:00:00",
            "EffectiveUntil": null
          }
        ]
      },
      {
        "Id": 4,
        "Name": "Cómputo de Descansos",
        "Description": "Regla diaria para computar tiempo de descanso",
        "Type": 3,
        "TypeDescription": "Daily",
        "GroupId": 1002,
        "Tags": ["descansos", "cómputo", "diaria"],
        "EditionStatus": 0,        
        "RuleDefinitions": [
          {
            "Id": 107,
            "Description": "Configuración inicial",
            "IdRule": 4,
            "EditionStatus": 0,
            "EmployeeContext": {
              "UserFields": "",
              "ComposeFilter": "All",
              "ComposeMode": "OR",
              "Filters": "",
              "Operation": "INCLUDE"
            },
            "Shifts": [
              {
                "IdShift": 43,
                "IdShiftGroup": 0,
                "IdRule": 4,
                "EditionStatus": 0,
                "Order": 1
              },
              {
                "IdShift": 63,
                "IdShiftGroup": 2,
                "EditionStatus": 0,
                "IdRule": 4,
                "Order": 2
              },
              {
                "IdShift": 66,
                "IdShiftGroup": 2,
                "EditionStatus": 0,
                "IdRule": 4,
                "Order": 3
              }
            ],
            "Conditions": [],
            "Actions": [],
            "XmlDefinition": "",
            "ModifiedBy": "supervisor",
            "ModifiedDate": "2025-01-25T11:45:00",
            "EffectiveFrom": "2025-02-01T00:00:00",
            "EffectiveUntil": "2025-05-14T23:59:59"
          },
          {
            "Id": 108,
            "Description": "Actualización descansos por convenio",
            "IdRule": 4,
            "EditionStatus": 0,
            "EmployeeContext": {
              "UserFields": "",
              "ComposeFilter": "All",
              "ComposeMode": "OR",
              "Filters": "",
              "Operation": "INCLUDE"
            },
            "Shifts": [
              {
                "IdShift": 43,
                "IdShiftGroup": 0,
                "IdRule": 4,
                "EditionStatus": 0,
                "Order": 1
              },
              {
                "IdShift": 63,
                "IdShiftGroup": 2,
                "EditionStatus": 0,
                "IdRule": 4,
                "Order": 2
              },
              {
                "IdShift": 66,
                "IdShiftGroup": 2,
                "EditionStatus": 0,
                "IdRule": 4,
                "Order": 3
              }
            ],
            "Conditions": [],
            "Actions": [],
            "XmlDefinition": "",
            "ModifiedBy": "supervisor",
            "ModifiedDate": "2025-05-10T13:20:00",
            "EffectiveFrom": "2025-05-15T00:00:00",
            "EffectiveUntil": "2025-09-30T23:59:59"
          },
          {
            "Id": 109,
            "Description": "Nueva política de descansos escalonados",
            "IdRule": 4,
            "EditionStatus": 0,
            "EmployeeContext": {
              "UserFields": "",
              "ComposeFilter": "All",
              "ComposeMode": "OR",
              "Filters": "",
              "Operation": "INCLUDE"
            },
            "Shifts": [
              {
                "IdShift": 43,
                "IdShiftGroup": 0,
                "IdRule": 4,
                "EditionStatus": 0,
                "Order": 1
              },
              {
                "IdShift": 63,
                "IdShiftGroup": 2,
                "EditionStatus": 0,
                "IdRule": 4,
                "Order": 2
              },
              {
                "IdShift": 66,
                "IdShiftGroup": 2,
                "EditionStatus": 0,
                "IdRule": 4,
                "Order": 3
              }
            ],
            "Conditions": [],
            "Actions": [],
            "XmlDefinition": "",
            "ModifiedBy": "hr_director",
            "ModifiedDate": "2025-09-20T11:15:00",
            "EffectiveFrom": "2025-10-01T00:00:00",
            "EffectiveUntil": null
          }
        ]
      },
      {
        "Id": 5,
        "Name": "Control de Asistencia",
        "Description": "Regla diaria para control de asistencia y puntualidad",
        "Type": 3,
        "TypeDescription": "Daily",
        "GroupId": 1002,
        "Tags": ["asistencia", "puntualidad", "diaria"],
        "EditionStatus": 0,        
        "RuleDefinitions": [
          {
            "Id": 110,
            "Description": "Configuración inicial",
            "IdRule": 5,
            "EditionStatus": 0,
            "EmployeeContext": {
              "UserFields": "Categoria=Operario",
              "ComposeFilter": "Category",
              "ComposeMode": "AND",
              "Filters": "Operario",
              "Operation": "INCLUDE"
            },
            "Shifts": [
              {
                "IdShift": 42,
                "IdShiftGroup": 0,
                "IdRule": 5,
                "EditionStatus": 0,
                "Order": 1
              },
              {
                "IdShift": 41,
                "IdShiftGroup": 1,
                "EditionStatus": 0,
                "IdRule": 5,
                "Order": 2
              }
            ],
            "Conditions": [],
            "Actions": [],
            "XmlDefinition": "",
            "ModifiedBy": "admin",
            "ModifiedDate": "2025-03-05T09:30:00",
            "EffectiveFrom": "2025-03-15T00:00:00",
            "EffectiveUntil": "2025-07-31T23:59:59"
          },
          {
            "Id": 111,
            "Description": "Actualización política flexibilidad",
            "IdRule": 5,
            "EditionStatus": 0,
            "EmployeeContext": {
              "UserFields": "Categoria=Operario",
              "ComposeFilter": "Category",
              "ComposeMode": "AND",
              "Filters": "Operario",
              "Operation": "INCLUDE"
            },
            "Shifts": [
              {
                "IdShift": 42,
                "IdShiftGroup": 0,
                "IdRule": 5,
                "EditionStatus": 0,
                "Order": 1
              },
              {
                "IdShift": 41,
                "IdShiftGroup": 1,
                "EditionStatus": 0,
                "IdRule": 5,
                "Order": 2
              }
            ],
            "Conditions": [],
            "Actions": [],
            "XmlDefinition": "",
            "ModifiedBy": "hr_manager",
            "ModifiedDate": "2025-07-20T10:15:00",
            "EffectiveFrom": "2025-08-01T00:00:00",
            "EffectiveUntil": null
          }
        ]
      }
    ]
  }
]'
)
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='1046' WHERE ID='DBVersion'
GO
