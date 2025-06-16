import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Link } from "react-router-dom";

export function PrivacyPolicyPage() {
  return (
    <div className="container mx-auto py-8 px-4 max-w-6xl space-y-6">
      <Card>
        <CardHeader>
          <CardTitle className="text-2xl font-bold">Privacy Policy</CardTitle>
        </CardHeader>
        <CardContent className="space-y-6">
          <section className="space-y-4">
            <h2 className="text-xl font-semibold">
              1. Data Collection and Processing
            </h2>
            <div className="space-y-2">
              <h3 className="text-lg font-medium">
                1.1 Personal Information We Collect
              </h3>
              <ul className="list-disc pl-6 space-y-1">
                <li>Basic Information: Name, contact details</li>
                <li>Account Information: Email, Username</li>     
              </ul>
            </div>

            <div className="space-y-2">
              <h3 className="text-lg font-medium">
                1.2 How We Process Your Data
              </h3>
              <ul className="list-disc pl-6 space-y-1">
                <li>Secure storage using industry-standard encryption</li>
                <li>Access limited to authorized professionals</li>
                <li>
                  Processing for Imgae detection and Waste Classification
                </li>
                <li>Anonymous data analysis for service improvement</li>
              </ul>
            </div>
          </section>

          <section className="space-y-4">
            <h2 className="text-xl font-semibold">2. Data Retention</h2>
            <div className="space-y-2">
              <h3 className="text-lg font-medium">2.1 Retention Periods</h3>
              <ul className="list-disc pl-6 space-y-1">
                <li>Account Information: Kept while account is active</li>
                <li>
                  Anonymized Data: May be retained indefinitely for research
                </li>
              </ul>
            </div>
          </section>

          <section className="space-y-4">
            <h2 className="text-xl font-semibold">3. Your Rights</h2>
            <div className="space-y-2">
              <h3 className="text-lg font-medium">
                3.1 You Have the Right to:
              </h3>
              <ul className="list-disc pl-6 space-y-1">
                <li>Access your personal data</li>
                <li>Request corrections to inaccurate information</li>
                <li>Object to certain processing activities</li>
                <li>Request data portability</li>
                <li>Lodge complaints with supervisory authorities</li>
              </ul>
            </div>

            <div className="space-y-2">
              <h3 className="text-lg font-medium">
                3.2 How to Exercise Your Rights
              </h3>
              <p>To exercise any of these rights, you can:</p>
              <ul className="list-disc pl-6 space-y-1">
                <li>Use the relevant options in your account settings</li>
                <li>
                  Contact our Data Protection Officer at privacy@hospital.com
                </li>
                <li>Submit a written request to our administrative office</li>
              </ul>
            </div>
          </section>

          <section className="space-y-4">
            <h2 className="text-xl font-semibold">4. Security Measures</h2>
            <div className="space-y-2">
              <p>
                We implement appropriate technical and organizational measures
                to ensure data security:
              </p>
              <ul className="list-disc pl-6 space-y-1">
                <li>Encryption of all sensitive data</li>
                <li>Regular security assessments</li>
                <li>Staff training on data protection</li>
                <li>Access controls and authentication</li>
              </ul>
            </div>
          </section>

          <section className="mt-6 text-sm text-muted-foreground">
            <p>Last updated: {new Date().toLocaleDateString()}</p>
            <p>
              For questions about this privacy policy, please contact our Data
              Protection Officer (contact in{" "}
              <Link
                to="/data-protection"
                className="text-blue-600 hover:underline"
              >
                Data Protection
              </Link>
              ).
            </p>
          </section>
        </CardContent>
      </Card>
    </div>
  );
}
