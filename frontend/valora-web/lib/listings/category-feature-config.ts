export type ListingFieldType = "text" | "number" | "select" | "checkbox";

export interface ListingFeatureFieldOption {
  label: string;
  value: string;
}

export interface ListingFeatureField {
  key: string;
  label: string;
  type: ListingFieldType;
  placeholder?: string;
  options?: ListingFeatureFieldOption[];
  min?: number;
  step?: number;
}

export interface ListingCategoryConfig {
  key: string;
  label: string;
  description: string;
  fields: ListingFeatureField[];
}

export const listingCategoryConfigs: ListingCategoryConfig[] = [
  {
    key: "laptops",
    label: "Laptops",
    description: "Voor gaming laptops, zakelijke laptops en ultrabooks.",
    fields: [
      { key: "brand", label: "Merk", type: "text", placeholder: "Bijv. Lenovo" },
      { key: "model", label: "Model", type: "text", placeholder: "Bijv. Legion 5" },
      { key: "cpu", label: "Processor", type: "text", placeholder: "Bijv. Intel i7" },
      { key: "ramGb", label: "RAM (GB)", type: "number", min: 1, step: 1 },
      { key: "storageGb", label: "Opslag (GB)", type: "number", min: 1, step: 1 },
      {
        key: "storageType",
        label: "Opslagtype",
        type: "select",
        options: [
          { label: "SSD", value: "SSD" },
          { label: "HDD", value: "HDD" },
          { label: "NVMe SSD", value: "NVMe SSD" },
        ],
      },
      { key: "screenSizeInch", label: "Schermgrootte", type: "number", min: 1, step: 0.1 },
      { key: "condition", label: "Staat", type: "text", placeholder: "Bijv. Zeer goed" },
    ],
  },
  {
    key: "phones",
    label: "Telefoons",
    description: "Voor smartphones zoals iPhone, Samsung of Google Pixel.",
    fields: [
      { key: "brand", label: "Merk", type: "text", placeholder: "Bijv. Apple" },
      { key: "model", label: "Model", type: "text", placeholder: "Bijv. iPhone 14 Pro" },
      { key: "storageGb", label: "Opslag (GB)", type: "number", min: 1, step: 1 },
      { key: "color", label: "Kleur", type: "text", placeholder: "Bijv. Zwart" },
      { key: "batteryHealthPercent", label: "Battery health %", type: "number", min: 1, step: 1 },
      {
        key: "simType",
        label: "Sim-type",
        type: "select",
        options: [
          { label: "Physical SIM", value: "Physical SIM" },
          { label: "eSIM", value: "eSIM" },
          { label: "Dual SIM", value: "Dual SIM" },
        ],
      },
      { key: "condition", label: "Staat", type: "text", placeholder: "Bijv. Goed" },
      { key: "hasBox", label: "Doos aanwezig", type: "checkbox" },
    ],
  },
  {
    key: "consoles",
    label: "Consoles",
    description: "Voor PlayStation, Xbox, Nintendo en vergelijkbare consoles.",
    fields: [
      { key: "brand", label: "Merk", type: "text", placeholder: "Bijv. Sony" },
      { key: "model", label: "Model", type: "text", placeholder: "Bijv. PlayStation 5" },
      { key: "storageGb", label: "Opslag (GB)", type: "number", min: 1, step: 1 },
      { key: "controllerCount", label: "Aantal controllers", type: "number", min: 0, step: 1 },
      { key: "color", label: "Kleur", type: "text", placeholder: "Bijv. Wit" },
      { key: "condition", label: "Staat", type: "text", placeholder: "Bijv. Goed" },
      { key: "includesGames", label: "Inclusief games", type: "checkbox" },
    ],
  },
];

export function getCategoryConfig(categoryKey: string): ListingCategoryConfig | undefined {
  return listingCategoryConfigs.find((category) => category.key === categoryKey);
}

export function formatCategoryLabel(categoryKey: string): string {
  return (
    getCategoryConfig(categoryKey)?.label ??
    categoryKey
      .replaceAll("-", " ")
      .replaceAll("_", " ")
      .replace(/\b\w/g, (char) => char.toUpperCase())
  );
}