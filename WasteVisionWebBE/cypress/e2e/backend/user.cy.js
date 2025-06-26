describe("User API E2E Tests", () => {
  // --- SETUP ---
  // Base URLs for the APIs
  const userBaseUrl = "http://localhost:3000/api/user";
  const roleBaseUrl = "http://localhost:3000/api/role"; // NEW: URL for Role API

  // These variables will be shared across all tests in this suite
  let userId;
  let roleId; // NEW: This will hold the ID of the role we create

  // --- `before()` HOOK FOR PREREQUISITES ---
  // This runs ONCE before any tests in this 'describe' block.
  // We create the Role that the User will depend on.
  before(() => {
    const testRole = {
      description: `E2E Test Role ${Date.now()}`,
    };

    cy.log("Creating prerequisite Role...");
    cy.request({
      method: "POST",
      url: roleBaseUrl,
      body: testRole,
    }).then((response) => {
      expect(response.status).to.eq(200);
      // Capture the created role's ID to use it when creating the user
      roleId = response.body.id;
      cy.log(`Role created successfully with ID: ${roleId}`);
    });
  });

  // --- CREATE (POST) ---
  it("should create a new user using the prerequisite roleId", () => {
    // Ensure the roleId was captured from the 'before' hook
    expect(roleId).to.not.be.undefined;

    const testUser = {
      email: `TestUser${Date.now()}@example.com`,
      username: "TestUserE2E",
      // MODIFIED: Use the dynamically created roleId
      roleId: roleId,
    };

    cy.request({
      method: "POST",
      url: userBaseUrl,
      body: testUser,
    }).then((response) => {
      expect(response.status).to.eq(200);
      const user = response.body;
      expect(user).to.have.property("email", testUser.email);
      expect(user).to.have.property("username", testUser.username);
      expect(user).to.have.property("roleId", roleId); // Assert against the dynamic roleId
      expect(user).to.have.property("id").and.to.be.a("string");

      // Store the user's ID for subsequent tests
      userId = user.id;
    });
  });

  it("should reject user creation with an invalid email format", () => {
    const invalidUser = {
      email: "invalid-email-format",
      username: "invalid_user",
      roleId: roleId, // Still need a valid roleId for the request
    };

    cy.request({
      method: "POST",
      url: userBaseUrl,
      body: invalidUser,
      failOnStatusCode: false,
    }).then((response) => {
      expect(response.status).to.eq(400);
    });
  });


  // --- READ (GET) ---
  it("should get a user by their ID", () => {
    expect(userId).to.not.be.undefined;
    cy.request("GET", `${userBaseUrl}/${userId}`).then((response) => {
      expect(response.status).to.eq(200);
      const user = response.body;
      expect(user).to.have.property("id", userId);
    });
  });

  // --- UPDATE (PUT) ---
  it("should update an existing user's details", () => {
    expect(userId).to.not.be.undefined;
    const updatedUserDetails = {
      email: `updated${Date.now()}@example.com`,
      username: "updated_username",
      roleId: roleId, // We can reuse the same roleId for the update
    };

    cy.request({
      method: "PUT",
      url: `${userBaseUrl}/${userId}`,
      body: updatedUserDetails,
    }).then((response) => {
      expect(response.status).to.eq(200);
      const user = response.body;
      expect(user).to.have.property("email", updatedUserDetails.email);
      expect(user).to.have.property("username", updatedUserDetails.username);
    });
  });

  // --- DEACTIVATE (SOFT DELETE) ---
  it("should deactivate (soft delete) a user", () => {
    expect(userId).to.not.be.undefined;
    cy.request("DELETE", `${userBaseUrl}/soft/${userId}`).then((response) => {
      expect(response.status).to.eq(200);
      expect(response.body).to.be.true;
    });
  });

   // --- ACTIVATE (PATCH) ---
  it("should activate the newly created user", () => {
    expect(userId).to.not.be.undefined;
    cy.request("PATCH", `${userBaseUrl}/activate/${userId}`).then((response) => {
      expect(response.status).to.eq(200);
      expect(response.body).to.be.true;
    });
  });

  // --- `after()` HOOK FOR CLEANUP ---
  // This runs ONCE after all tests in this 'describe' block are complete.
  // We clean up both the User and the Role to keep the database clean.
  after(() => {
    // IMPORTANT: Delete the user BEFORE deleting the role it depends on.
    if (userId) {
      cy.log(`Cleaning up User with ID: ${userId}`);

      cy.request({
        method: "DELETE",
        url: `${userBaseUrl}/soft/${userId}`,
        failOnStatusCode: false, // Don't fail the test if cleanup fails
      });

      cy.request({
        method: "DELETE",
        url: `${userBaseUrl}/hard/${userId}`,
        failOnStatusCode: false, // Don't fail the test if cleanup fails
      });
    }

    if (roleId) {
      cy.log(`Cleaning up Role with ID: ${roleId}`);
      cy.request({
        method: "DELETE",
        // Assuming your role delete endpoint is /api/role/{id}
        url: `${roleBaseUrl}/${roleId}`,
        failOnStatusCode: false,
      });
    }
  });
});