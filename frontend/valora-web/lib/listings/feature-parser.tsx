export interface ParsedFeature {
  key: string;
  label: string;
  value: string;
}

function toLabel(key: string): string {
  return key
    .replace(/([a-z])([A-Z])/g, "$1 $2")
    .replaceAll("-", " ")
    .replaceAll("_", " ")
    .replace(/\b\w/g, (char) => char.toUpperCase());
}

function normalizeValue(value: unknown): string {
  if (value === null || value === undefined) {
    return "Unknown";
  }

  if (typeof value === "boolean") {
    return value ? "Yes" : "No";
  }

  if (Array.isArray(value)) {
    return value.map((item) => String(item)).join(", ");
  }

  if (typeof value === "object") {
    return JSON.stringify(value);
  }

  return String(value);
}

export function parseFeaturesJson(featuresJson: string): ParsedFeature[] {
  if (!featuresJson || !featuresJson.trim()) {
    return [];
  }

  try {
    const parsed = JSON.parse(featuresJson) as Record<string, unknown>;

    return Object.entries(parsed)
      .filter(([, value]) => value !== null && value !== undefined && value !== "")
      .map(([key, value]) => ({
        key,
        label: toLabel(key),
        value: normalizeValue(value),
      }));
  } catch {
    return [];
  }
}

export function getTopFeatureBadges(
  featuresJson: string,
  maxCount = 4,
): ParsedFeature[] {
  return parseFeaturesJson(featuresJson).slice(0, maxCount);
}

export function parseFeatureObject(
  featuresJson: string,
): Record<string, string | number | boolean> {
  if (!featuresJson || !featuresJson.trim()) {
    return {};
  }

  try {
    const parsed = JSON.parse(featuresJson) as Record<string, unknown>;

    return Object.fromEntries(
      Object.entries(parsed).filter(
        ([, value]) =>
          typeof value === "string" ||
          typeof value === "number" ||
          typeof value === "boolean",
      ),
    ) as Record<string, string | number | boolean>;
  } catch {
    return {};
  }
}