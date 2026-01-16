export default class DeleteAfter {
  static readonly Viewing = new DeleteAfter(0, "Viewing");
  static readonly OneHour = new DeleteAfter(1, "An hour");
  static readonly OneDay = new DeleteAfter(2, "A day");
  static readonly OneWeek = new DeleteAfter(3, "A week");

  private constructor(
    readonly value: number,
    readonly text: string,
  ) {}

  public static listAll(): DeleteAfter[] {
    return [DeleteAfter.Viewing, DeleteAfter.OneHour, DeleteAfter.OneDay, DeleteAfter.OneWeek];
  }

  public static fromValue(value: number) {
    const matches = DeleteAfter.listAll().filter((x) => x.value === value);

    if (matches.length === 0) {
      throw new Error(`Unknown delete after value: ${value}`);
    }

    return matches[0];
  }
}
