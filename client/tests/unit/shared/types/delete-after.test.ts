import DeleteAfter from "@/shared/types/delete-after";
import { describe, expect, it } from "vitest";

describe("DeleteAfter", () => {
  describe("listAll", () => {
    it("returns all four options", () => {
      const all = DeleteAfter.listAll();
      expect(all).toHaveLength(4);
    });

    it("contains Viewing, OneHour, OneDay, and OneWeek", () => {
      const all = DeleteAfter.listAll();
      expect(all).toContain(DeleteAfter.Viewing);
      expect(all).toContain(DeleteAfter.OneHour);
      expect(all).toContain(DeleteAfter.OneDay);
      expect(all).toContain(DeleteAfter.OneWeek);
    });
  });

  describe("fromValue", () => {
    it("returns the correct instance for each known value", () => {
      expect(DeleteAfter.fromValue(0)).toBe(DeleteAfter.Viewing);
      expect(DeleteAfter.fromValue(1)).toBe(DeleteAfter.OneHour);
      expect(DeleteAfter.fromValue(2)).toBe(DeleteAfter.OneDay);
      expect(DeleteAfter.fromValue(3)).toBe(DeleteAfter.OneWeek);
    });

    it("throws for an unknown value", () => {
      expect(() => DeleteAfter.fromValue(99)).toThrow("Unknown delete after value: 99");
    });
  });

  describe("properties", () => {
    it("exposes value and text", () => {
      expect(DeleteAfter.Viewing.value).toBe(0);
      expect(DeleteAfter.Viewing.text).toBe("Viewing");
      expect(DeleteAfter.OneWeek.value).toBe(3);
      expect(DeleteAfter.OneWeek.text).toBe("A week");
    });
  });
});
