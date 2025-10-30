export function logout(): Promise<void> {
    return new Cypress.Promise((resolve, reject) => {
        cy.request({
            method: 'GET',
            url: Cypress.env("baseUrlPortal") + '/'
            }).then((response) => {
                if (response.status === 200){
                    resolve()
                } else {
                    reject(new Error('Error when calling tc_logout.jsp.'));
                }
            })
    })
}