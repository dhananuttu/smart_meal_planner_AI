import { FormEvent, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { Button, Input, Label } from "@/components/ui";
import { AuthCard } from "./AuthCard";
import { useAuth } from "@/hooks/useAuth";
import { getApiErrorMessage } from "@/utils/apiError";

export function RegisterForm() {
  const navigate = useNavigate();
  const { register, isRegistering } = useAuth();
  const [displayName, setDisplayName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError(null);

    try {
      await register({
        displayName: displayName.trim(),
        email: email.trim(),
        password,
      });
      navigate("/dashboard", { replace: true });
    } catch (err) {
      setError(getApiErrorMessage(err, "Unable to create your account."));
    }
  }

  return (
    <AuthCard
      title="Create your account"
      subtitle="Start planning meals for your household in minutes."
      footer={
        <p className="text-sm text-slate-600">
          Already have an account?{" "}
          <Link className="font-medium text-brand-700 hover:text-brand-600" to="/login">
            Sign in
          </Link>
        </p>
      }
    >
      <form className="space-y-5" onSubmit={handleSubmit}>
        {error ? (
          <div
            className="rounded-lg border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-700"
            role="alert"
          >
            {error}
          </div>
        ) : null}

        <div>
          <Label htmlFor="displayName">Display name</Label>
          <Input
            id="displayName"
            type="text"
            autoComplete="name"
            required
            maxLength={100}
            value={displayName}
            onChange={(event) => setDisplayName(event.target.value)}
            placeholder="Alex Cook"
          />
        </div>

        <div>
          <Label htmlFor="email">Email</Label>
          <Input
            id="email"
            type="email"
            autoComplete="email"
            required
            value={email}
            onChange={(event) => setEmail(event.target.value)}
            placeholder="you@example.com"
          />
        </div>

        <div>
          <Label htmlFor="password">Password</Label>
          <Input
            id="password"
            type="password"
            autoComplete="new-password"
            required
            minLength={8}
            value={password}
            onChange={(event) => setPassword(event.target.value)}
            placeholder="At least 8 characters"
          />
          <p className="mt-1.5 text-xs text-slate-500">
            Must include uppercase, lowercase, and a number.
          </p>
        </div>

        <Button type="submit" className="w-full" isLoading={isRegistering}>
          Create account
        </Button>
      </form>
    </AuthCard>
  );
}
