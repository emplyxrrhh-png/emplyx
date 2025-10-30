const baseHelper = require("../../../helpers/baseHelper");




describe.skip('ConexionSQL - Acceso a Base de Datos', () => {
  it('Seleccionar Empleados', baseHelper.retriesDefault, () => {
    cy.log("HOLA")

    cy.sqlServer('SELECT ID,Name FROM Employees where ID<130').then(result => {
      if (result.length > 0) {
        let columnNames = Object.keys(result[0]);
        cy.log(columnNames); // ['ID', 'FirstName', 'LastName']
        cy.log(JSON.stringify(columnNames));
      }
      cy.log(JSON.stringify(result));

      let object = result.reduce((obj, row) => {
        obj[row.ID] = row;
        return obj;
      }, {});

      for (let id in object) {
        let row = object[id];
        // Haz algo con la fila
        cy.log(row)
      }

    });
  })
});

