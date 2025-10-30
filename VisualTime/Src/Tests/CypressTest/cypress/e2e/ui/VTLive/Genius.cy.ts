import { method } from "cypress/types/lodash";
import * as loginHelper from "../../../helpers/loginHelper";

var estudioSupervisoresId = 73;
var estudioSaldosId = 74;
var estudioFichajesId = 75;
var estudioFichajesSaldosId = 76;

describe('Genius Pantalla-', () => {
    beforeEach(() => {
        loginHelper.login('g.loco', '12345678As+');
        cy.intercept('GET', 'https://vtliveidi.azurewebsites.net/Genius/GetPlanningList?*').as('GetPlanningList');
        cy.window().then((win) => {
            win.reenviaFrame('/Genius.aspx', '', '', '');
        });

        cy.wait('@GetPlanningList').its('response.statusCode').should('eq', 200);
        cy.wait(5000);
    });

    afterEach(() => {
        loginHelper.logout();
    });

    it('Test para verificar que el usuario pueda ingresar al sistema y que los Iframes estén cargados accediendo a la sección de Geniuns.', () => {
        cy.getIframeBody('#ifPrincipal').find('#ctl00_contentMainBody_lblHeader').invoke('text').then((text) => {
            cy.wrap(text.trim()).should('equal', "Analytics by Genius".trim());
        });
    });

    it('El estudio de Supervisores es encontrado', () => {
        cy.getIframeBody('#ifPrincipal').find('[data-card-id="73"]>h3').invoke('text').then((text) => {
            cy.wrap(text.trim()).should('equal', "Supervisores".trim());
        });
    });
});


describe('Genius Callbacks-', () => {

    function waitForFileToBeReady(id: string, retries: number = 10) {
        if (retries <= 0) {
            throw new Error('El archivo no estuvo listo dentro del tiempo esperado.');
        }

        cy.request({
            method: 'POST',
            url: 'https://vtliveidi.azurewebsites.net/Genius/getGeniusViewStatus',
            body: `id=${id}`,
            form: true,
        }).then((response) => {

            cy.log('Response JSON getGeniusViewStatus :', JSON.stringify(response.body, null, 2));

            expect(response.status).to.eq(200);
            // Verifica si el archivo está listo
            if (response.body == true) {
                cy.log('El archivo está listo.');
            } else {
                cy.log('El archivo aún no está listo. Reintentando...');
                cy.wait(2000); // Espera 2 segundos antes de volver a intentarlo
                waitForFileToBeReady(id, retries - 1); // Llama a la función de nuevo
            }
        });
    }



    beforeEach(() => {
        loginHelper.login('g.loco', '12345678As+')
        cy.intercept('GET', 'https://vtliveidi.azurewebsites.net/Genius/GetPlanningList?*').as('GetPlanningList');
        cy.window().then((win) => {
            win.reenviaFrame('/Genius.aspx', '', '', '');
        });

        cy.wait('@GetPlanningList').its('response.statusCode').should('eq', 200);
        cy.wait(5000)
    });

    afterEach(() => {
        try{
        loginHelper.logout()
        }catch(error){  
            cy.log('Error durante el logout:', error.message);
        }
    });

    // it('Ejemplo de prueba con manejo de errores', () => {
    //     cy.log('Error capturado 2:', 'Error de prueba');
    //     cy.wrap(5).then((value) => {
    //         try {
    //             expect(value).to.equal(10); // Esto fallará
    //         } catch (error) {
    //             cy.log('Error capturado:', error.message);
    //             assert.fail(`El valor esperado era 10, pero se obtuvo ${value}`);
              
    //         }
    //     });

    //     try {
    //         expect(6).to.equal(10); // Esto fallará
    //     } catch (error) {
    //         cy.log('Error capturado 2:', error.message);
            
    //     }

    //     cy.log('Este log se ejecutará incluso si la aserción anterior falla');
    //     cy.wrap(10).then((value) => {
    //         expect(value).to.equal(10); // Esto se ejecutará
    //     });
    // });

    it('El estudio de Supervisores es lanzando', () => {
        cy.log("Lanzando el estudio de supervisores")
        cy.request({
            method: 'POST',
            url: "https://vtliveidi.azurewebsites.net/Genius/runGeniusView",
            body: "geniusView%5BId%5D=" + estudioSupervisoresId + "&geniusView%5BName%5D=Supervisores&geniusView%5BDescription%5D=&geniusView%5BDS%5D=7&geniusView%5BTypeView%5D=1&geniusView%5BCreatedOn%5D=Wed+Feb+26+2025+09%3A14%3A58+GMT%2B0100+(hora+est%C3%A1ndar+de+Europa+central)&geniusView%5BIdPassport%5D=20&geniusView%5BIdParentShared%5D=-1&geniusView%5BEmployees%5D=A1%2311110%23&geniusView%5BConcepts%5D=&geniusView%5BCauses%5D=&geniusView%5BDateFilterType%5D=0&geniusView%5BDateInf%5D=2025-02-26T00%3A00%3A00.000Z&geniusView%5BDateSup%5D=2025-02-26T00%3A00%3A00.000Z&geniusView%5BTimeInf%5D=&geniusView%5BTimeSup%5D=&geniusView%5BCubeLayout%5D=%7B%22slice%22%3A%7B%22rows%22%3A%5B%7B%22uniqueName%22%3A%221%22%7D%2C%7B%22uniqueName%22%3A%222%22%7D%2C%7B%22uniqueName%22%3A%229%22%7D%2C%7B%22uniqueName%22%3A%2210%22%7D%5D%2C%22columns%22%3A%5B%7B%22uniqueName%22%3A%22%5BMeasures%5D%22%7D%5D%2C%22measures%22%3A%5B%7B%22uniqueName%22%3A%2211%22%2C%22aggregation%22%3A%22count%22%2C%22caption%22%3A%22Activo%22%7D%5D%2C%22expands%22%3A%7B%22expandAll%22%3Atrue%7D%7D%2C%22options%22%3A%7B%22grid%22%3A%7B%22type%22%3A%22classic%22%2C%22showTotals%22%3A%22off%22%7D%7D%2C%22creationDate%22%3A%222022-02-14T15%3A18%3A40.046Z%22%7D&geniusView%5BUserFields%5D=&geniusView%5BBusinessCenters%5D=&geniusView%5BCustomFields%5D%5BIncludeZeros%5D=false&geniusView%5BCustomFields%5D%5BIncludeZeroCauses%5D=false&geniusView%5BCustomFields%5D%5BIncludeZeroBusinessCenter%5D=false&geniusView%5BCustomFields%5D%5BLanguageKey%5D=ESP&geniusView%5BCustomFields%5D%5BRequestTypes%5D=1%2C2%2C3%2C4%2C5%2C6%2C7%2C8%2C9%2C10%2C11%2C12%2C13%2C14%2C15&geniusView%5BCustomFields%5D%5BSendEmail%5D=false&geniusView%5BCustomFields%5D%5BOverwriteResults%5D=false&geniusView%5BCustomFields%5D%5BDownloadBI%5D=false&geniusView%5BDSFunction%5D=Genius_Supervisors(%40initialDate%2C%40endDate%2C%40idpassport%2C%40employeeFilter%2C%40userFieldsFilter)&geniusView%5BFeature%5D=Employees&geniusView%5BRequieredFeature%5D=Employees&geniusView%5BRequieredLicense%5D=&geniusView%5BIdSystemView%5D=61&geniusView%5BContextMenu%5D=",
            form: true,
        }).then(
            (response) => {
                cy.log('Response JSON:', JSON.stringify(response.body, null, 2));
                // response.body is automatically serialized into JSON
                cy.wrap(response.status).should('eq', 200);
                cy.wrap(response.body).should('have.property', 'IdGeniusView', estudioSupervisoresId.toString());
                waitForFileToBeReady(response.body.Id)
                cy.request({
                    method: 'POST',
                    url: "https://vtliveidi.azurewebsites.net/Genius/GetAzureFileInfo",
                    body: "id=" + response.body.Id,
                    form: true,
                    timeout: 60000, // Tiempo de espera extendido a 60 segundos
                }).then(
                    (response) => {

                        cy.log('Response JSON:', JSON.stringify(response.body, null, 2));
                        // response.body is automatically serialized into JSON
                        try {
                            cy.wrap(response.status).should('eq', 200);
                            cy.wrap(response.body).should('have.property', 'IdGeniusView', estudioSupervisoresId.toString());
                            cy.wrap(response.body).its('FileSize').should('be.greaterThan', 0);
                            cy.wrap(response.body).should('have.property', 'FileSize', 1721); // IgualTamaño
                        } catch (error) {
                            assert.fail('Error capturado:', error.message);
                        }

                        //AzureSaSKey propiedad para ver el fichero es una URL por lo que hay que hacer llamada hacia el arichivo para obtener el binario.
                    }
                )



            }
        )

    })

    it('El estudio de Saldos es lanzando', () => {
        //Generamos el estudio de saldos
        cy.request({
            method: 'POST',
            url: "https://vtliveidi.azurewebsites.net/Genius/runGeniusView",
            body: "geniusView%5BId%5D=" + estudioSaldosId + "&geniusView%5BName%5D=Estudio+de+Saldos+2024&geniusView%5BDescription%5D=&geniusView%5BDS%5D=2&geniusView%5BTypeView%5D=1&geniusView%5BCreatedOn%5D=Wed+Mar+26+2025+07%3A52%3A58+GMT%2B0100+(hora+est%C3%A1ndar+de+Europa+central)&geniusView%5BIdPassport%5D=20&geniusView%5BIdParentShared%5D=-1&geniusView%5BEmployees%5D=A1%2311110%23&geniusView%5BConcepts%5D=80%2C79%2C81%2C77%2C76&geniusView%5BCauses%5D=&geniusView%5BDateFilterType%5D=0&geniusView%5BDateInf%5D=2024-01-01T00%3A00%3A00.000Z&geniusView%5BDateSup%5D=2024-12-31T00%3A00%3A00.000Z&geniusView%5BTimeInf%5D=&geniusView%5BTimeSup%5D=&geniusView%5BCubeLayout%5D=%7B%22slice%22%3A%7B%22reportFilters%22%3A%5B%7B%22uniqueName%22%3A%223%22%7D%2C%7B%22uniqueName%22%3A%2220%22%7D%5D%2C%22rows%22%3A%5B%7B%22uniqueName%22%3A%225%22%7D%2C%7B%22uniqueName%22%3A%226%22%7D%2C%7B%22uniqueName%22%3A%227%22%7D%2C%7B%22uniqueName%22%3A%2210%22%7D%5D%2C%22columns%22%3A%5B%7B%22uniqueName%22%3A%22%5BMeasures%5D%22%7D%2C%7B%22uniqueName%22%3A%228%22%7D%5D%2C%22measures%22%3A%5B%7B%22uniqueName%22%3A%22Hours_ToHours%22%2C%22formula%22%3A%22sum(%5C%2211%5C%22)%22%2C%22caption%22%3A%22Hours_ToHours%22%7D%5D%2C%22expands%22%3A%7B%22expandAll%22%3Atrue%7D%7D%2C%22options%22%3A%7B%22grid%22%3A%7B%22type%22%3A%22classic%22%2C%22showTotals%22%3A%22off%22%2C%22showGrandTotals%22%3A%22columns%22%7D%7D%2C%22creationDate%22%3A%222021-10-07T09%3A43%3A29.791Z%22%7D&geniusView%5BUserFields%5D=Id+importaci%C3%B3n&geniusView%5BBusinessCenters%5D=&geniusView%5BCustomFields%5D%5BIncludeZeros%5D=false&geniusView%5BCustomFields%5D%5BIncludeZeroCauses%5D=false&geniusView%5BCustomFields%5D%5BIncludeZeroBusinessCenter%5D=true&geniusView%5BCustomFields%5D%5BLanguageKey%5D=ESP&geniusView%5BCustomFields%5D%5BRequestTypes%5D=&geniusView%5BCustomFields%5D%5BSendEmail%5D=false&geniusView%5BCustomFields%5D%5BOverwriteResults%5D=false&geniusView%5BCustomFields%5D%5BDownloadBI%5D=false&geniusView%5BDSFunction%5D=Genius_Concepts(%40initialDate%2C%40endDate%2C%40idpassport%2C%40employeeFilter%2C%40conceptsFilter%2C%40userFieldsFilter%2C%40includeZeros)&geniusView%5BFeature%5D=Calendar&geniusView%5BRequieredFeature%5D=Calendar.Analytics&geniusView%5BRequieredLicense%5D=&geniusView%5BCheckedCheckBoxes%5D%5B%5D=6&geniusView%5BIdSystemView%5D=33&geniusView%5BContextMenu%5D=",
            form: true,
        }).then(
            (response) => {

                cy.log('Response JSON:', JSON.stringify(response.body, null, 2));

                cy.wrap(response.status).should('eq', 200);
                cy.wrap(response.body).should('have.property', 'IdGeniusView', estudioSaldosId.toString());

                //Eperamos a la generacion del archivo en azure
                waitForFileToBeReady(response.body.Id)

                cy.request({
                    method: 'POST',
                    url: "https://vtliveidi.azurewebsites.net/Genius/GetAzureFileInfo",
                    body: "id=" + response.body.Id,
                    form: true,
                    timeout: 60000, // Tiempo de espera extendido a 60 segundos
                }).then(
                    (response) => {

                        cy.log('Response JSON:', JSON.stringify(response.body, null, 2));

                        try {
                            cy.wrap(response.status).should('eq', 200);
                            cy.wrap(response.body).should('have.property', 'IdGeniusView', estudioSaldosId.toString());
                            cy.wrap(response.body).its('FileSize').should('be.greaterThan', 0);
                            cy.wrap(response.body).should('have.property', 'FileSize', 33224); // IgualTamaño
                        } catch (error) {
                            assert.fail('Error capturado:', error.message);
                        }
                        //AzureSaSKey propiedad para ver el fichero es una URL por lo que hay que hacer llamada hacia el arichivo para obtener el binario.
                    }
                )



            }
        )
    })

    it('El estudio de Fichajes es lanzando', () => {

        cy.request({
            method: 'POST',
            url: "https://vtliveidi.azurewebsites.net/Genius/runGeniusView",
            body: "geniusView%5BId%5D=" + estudioFichajesId + "&geniusView%5BName%5D=Estudio+Fichajes+2024&geniusView%5BDescription%5D=&geniusView%5BDS%5D=8&geniusView%5BTypeView%5D=1&geniusView%5BCreatedOn%5D=Wed+Mar+26+2025+12%3A36%3A24+GMT%2B0100+(hora+est%C3%A1ndar+de+Europa+central)&geniusView%5BIdPassport%5D=20&geniusView%5BIdParentShared%5D=-1&geniusView%5BEmployees%5D=B7%2CB8%2311110%23&geniusView%5BConcepts%5D=&geniusView%5BCauses%5D=&geniusView%5BDateFilterType%5D=0&geniusView%5BDateInf%5D=2024-01-01T00%3A00%3A00.000Z&geniusView%5BDateSup%5D=2024-01-31T00%3A00%3A00.000Z&geniusView%5BTimeInf%5D=&geniusView%5BTimeSup%5D=&geniusView%5BCubeLayout%5D=%7B%22slice%22%3A%7B%22reportFilters%22%3A%5B%7B%22uniqueName%22%3A%226%22%7D%2C%7B%22uniqueName%22%3A%2228%22%7D%2C%7B%22uniqueName%22%3A%2258%22%7D%2C%7B%22uniqueName%22%3A%2259%22%7D%2C%7B%22uniqueName%22%3A%2260%22%7D%2C%7B%22uniqueName%22%3A%2261%22%7D%2C%7B%22uniqueName%22%3A%2262%22%7D%2C%7B%22uniqueName%22%3A%2263%22%7D%2C%7B%22uniqueName%22%3A%2264%22%7D%2C%7B%22uniqueName%22%3A%2265%22%7D%2C%7B%22uniqueName%22%3A%2266%22%7D%2C%7B%22uniqueName%22%3A%2267%22%7D%5D%2C%22rows%22%3A%5B%7B%22uniqueName%22%3A%222%22%7D%2C%7B%22uniqueName%22%3A%223%22%7D%2C%7B%22uniqueName%22%3A%225%22%7D%2C%7B%22uniqueName%22%3A%2213%22%7D%2C%7B%22uniqueName%22%3A%2214%22%7D%2C%7B%22uniqueName%22%3A%2223%22%7D%2C%7B%22uniqueName%22%3A%22PDDesc%22%7D%5D%2C%22columns%22%3A%5B%7B%22uniqueName%22%3A%22%5BMeasures%5D%22%7D%5D%2C%22measures%22%3A%5B%7B%22uniqueName%22%3A%2219%22%2C%22aggregation%22%3A%22sum%22%2C%22caption%22%3A%22Valor%22%7D%5D%2C%22expands%22%3A%7B%22expandAll%22%3Atrue%7D%7D%2C%22options%22%3A%7B%22grid%22%3A%7B%22type%22%3A%22classic%22%2C%22showTotals%22%3A%22off%22%7D%7D%2C%22creationDate%22%3A%222021-04-19T15%3A46%3A46.872Z%22%7D&geniusView%5BUserFields%5D=Id+importaci%C3%B3n&geniusView%5BBusinessCenters%5D=&geniusView%5BCustomFields%5D%5BIncludeZeros%5D=false&geniusView%5BCustomFields%5D%5BIncludeZeroCauses%5D=false&geniusView%5BCustomFields%5D%5BIncludeZeroBusinessCenter%5D=true&geniusView%5BCustomFields%5D%5BLanguageKey%5D=ESP&geniusView%5BCustomFields%5D%5BRequestTypes%5D=&geniusView%5BCustomFields%5D%5BSendEmail%5D=false&geniusView%5BCustomFields%5D%5BOverwriteResults%5D=false&geniusView%5BCustomFields%5D%5BDownloadBI%5D=false&geniusView%5BDSFunction%5D=Genius_Punches(%40initialDate%2C%40endDate%2C%40idpassport%2C%40employeeFilter%2C%40userFieldsFilter)&geniusView%5BFeature%5D=Calendar&geniusView%5BRequieredFeature%5D=Calendar.Analytics&geniusView%5BRequieredLicense%5D=&geniusView%5BCheckedCheckBoxes%5D%5B%5D=2&geniusView%5BIdSystemView%5D=35&geniusView%5BContextMenu%5D=",
            form: true,
        }).then(
            (response) => {
                cy.log('Response JSON:', JSON.stringify(response.body, null, 2));
                // response.body is automatically serialized into JSON

                cy.wrap(response.status).should('eq', 200);
                cy.wrap(response.body).should('have.property', 'IdGeniusView', estudioFichajesId.toString());

                waitForFileToBeReady(response.body.Id)

                cy.request({
                    method: 'POST',
                    url: "https://vtliveidi.azurewebsites.net/Genius/GetAzureFileInfo",
                    body: "id=" + response.body.Id,
                    form: true,
                    timeout: 60000, // Tiempo de espera extendido a 60 segundos
                }).then(
                    (response) => {
                        cy.log('Response JSON:', JSON.stringify(response.body, null, 2));
                        // response.body is automatically serialized into JSON
                        try {
                            cy.wrap(response.status).should('eq', 200);
                            cy.wrap(response.body).should('have.property', 'IdGeniusView', estudioFichajesId.toString());
                            cy.wrap(response.body).its('FileSize').should('be.greaterThan', 0);
                            cy.wrap(response.body).should('have.property', 'FileSize', 21809); // IgualTamaño
                        } catch (error) {
                            assert.fail('Error capturado:', error.message);
                        }
                        //AzureSaSKey propiedad para ver el fichero es una URL por lo que hay que hacer llamada hacia el arichivo para obtener el binario.
                    }
                )



            }
        )
    })

    it('El estudio de Fichajes+Saldos es lanzando', () => {

        cy.request({
            method: 'POST',
            url: "https://vtliveidi.azurewebsites.net/Genius/runGeniusView",
            body: "geniusView%5BId%5D=" + estudioFichajesSaldosId + "&geniusView%5BName%5D=Estudio+de+Saldos%2BFichajes&geniusView%5BDescription%5D=&geniusView%5BDS%5D=2&geniusView%5BTypeView%5D=1&geniusView%5BCreatedOn%5D=Thu+Apr+10+2025+13%3A22%3A58+GMT%2B0200+(hora+de+verano+de+Europa+central)&geniusView%5BIdPassport%5D=20&geniusView%5BIdParentShared%5D=-1&geniusView%5BEmployees%5D=B7%2CB8%2311110%23&geniusView%5BConcepts%5D=81&geniusView%5BCauses%5D=&geniusView%5BDateFilterType%5D=0&geniusView%5BDateInf%5D=2024-01-01T00%3A00%3A00.000Z&geniusView%5BDateSup%5D=2024-12-31T00%3A00%3A00.000Z&geniusView%5BTimeInf%5D=&geniusView%5BTimeSup%5D=&geniusView%5BCubeLayout%5D=%7B%22slice%22%3A%7B%22reportFilters%22%3A%5B%7B%22uniqueName%22%3A%223%22%7D%2C%7B%22uniqueName%22%3A%2216%22%7D%2C%7B%22uniqueName%22%3A%2235%22%7D%2C%7B%22uniqueName%22%3A%2236%22%7D%2C%7B%22uniqueName%22%3A%2237%22%7D%2C%7B%22uniqueName%22%3A%2238%22%7D%2C%7B%22uniqueName%22%3A%2239%22%7D%2C%7B%22uniqueName%22%3A%2240%22%7D%2C%7B%22uniqueName%22%3A%2241%22%7D%2C%7B%22uniqueName%22%3A%2242%22%7D%2C%7B%22uniqueName%22%3A%2243%22%7D%2C%7B%22uniqueName%22%3A%2244%22%7D%5D%2C%22rows%22%3A%5B%7B%22uniqueName%22%3A%225%22%7D%2C%7B%22uniqueName%22%3A%226%22%7D%2C%7B%22uniqueName%22%3A%227%22%7D%2C%7B%22uniqueName%22%3A%2248%22%7D%5D%2C%22columns%22%3A%5B%7B%22uniqueName%22%3A%22%5BMeasures%5D%22%7D%5D%2C%22measures%22%3A%5B%7B%22uniqueName%22%3A%22dynamic_concepts%22%2C%22aggregation%22%3A%22count%22%7D%5D%2C%22expands%22%3A%7B%22expandAll%22%3Atrue%7D%7D%2C%22options%22%3A%7B%22grid%22%3A%7B%22type%22%3A%22classic%22%2C%22showTotals%22%3A%22off%22%7D%7D%2C%22creationDate%22%3A%222021-06-28T07%3A53%3A55.468Z%22%7D&geniusView%5BUserFields%5D=Correo+electr%C3%B3nico%2CFecha+de+nacimiento&geniusView%5BBusinessCenters%5D=&geniusView%5BCustomFields%5D%5BIncludeZeros%5D=false&geniusView%5BCustomFields%5D%5BIncludeZeroCauses%5D=false&geniusView%5BCustomFields%5D%5BIncludeZeroBusinessCenter%5D=true&geniusView%5BCustomFields%5D%5BLanguageKey%5D=ESP&geniusView%5BCustomFields%5D%5BRequestTypes%5D=&geniusView%5BCustomFields%5D%5BSendEmail%5D=false&geniusView%5BCustomFields%5D%5BOverwriteResults%5D=false&geniusView%5BCustomFields%5D%5BDownloadBI%5D=false&geniusView%5BDSFunction%5D=Genius_ConceptsAndPunches(%40initialDate%2C%40endDate%2C%40idpassport%2C%40employeeFilter%2C%40conceptsFilter%2C%40userFieldsFilter%2C%40includeZeros)&geniusView%5BFeature%5D=Calendar&geniusView%5BRequieredFeature%5D=Calendar.Analytics&geniusView%5BRequieredLicense%5D=&geniusView%5BCheckedCheckBoxes%5D%5B%5D=2&geniusView%5BCheckedCheckBoxes%5D%5B%5D=6&geniusView%5BIdSystemView%5D=36&geniusView%5BContextMenu%5D=",
            form: true,
        }).then(
            (response) => {
                cy.log('Response JSON:', JSON.stringify(response.body, null, 2));
                // response.body is automatically serialized into JSON

                cy.wrap(response.status).should('eq', 200);
                cy.wrap(response.body).should('have.property', 'IdGeniusView', estudioFichajesSaldosId.toString());
                waitForFileToBeReady(response.body.Id)

                cy.request({
                    method: 'POST',
                    url: "https://vtliveidi.azurewebsites.net/Genius/GetAzureFileInfo",
                    body: "id=" + response.body.Id,
                    form: true,
                    timeout: 60000, // Tiempo de espera extendido a 60 segundos
                }).then(
                    (response) => {
                        cy.log('Response JSON:', JSON.stringify(response.body, null, 2));
                        // response.body is automatically serialized into JSON
                        try {
                            cy.wrap(response.status).should('eq', 200);
                            cy.wrap(response.body).should('have.property', 'IdGeniusView', estudioFichajesSaldosId.toString());
                            cy.wrap(response.body).its('FileSize').should('be.greaterThan', 0);
                            cy.wrap(response.body).should('have.property', 'FileSize', 6835); // IgualTamaño
                        } catch (error) {
                            assert.fail('Error capturado:', error.message);
                        }
                        //AzureSaSKey propiedad para ver el fichero es una URL por lo que hay que hacer llamada hacia el arichivo para obtener el binario.
                    }
                )
            }
        )
    })
});